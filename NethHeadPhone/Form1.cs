using SharpLib.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
//For ClickOnce support
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Security.Cryptography;
using Hid = SharpLib.Hid;

namespace NethHeadPhone
{
    public partial class HeadPhone : Form
    {
        public System.Windows.Forms.NotifyIcon notifyIcon1;
        private Hid.Handler iHidHandler;
        private Hid.Handler iHidParser;
        public delegate void OnHidEventDelegate(object aSender, Hid.Event aHidEvent);

        public string username = "";
        public string password = "";
        public string host = "";
        public string Token = "";

        string Ext;
        string Type;

        public HeadPhone()
        {
            InitializeComponent();
            
            foreach(string Arg in Environment.GetCommandLineArgs()) {
                if (Arg.ToLower().StartsWith("-username=")) {
                    username = Arg.Remove(0, "-username=".Length);
                } else if (Arg.ToLower().StartsWith("-password=")) { 
                    password = Arg.Remove(0, "-password=".Length);
                } else if (Arg.ToLower().StartsWith("-host=")) {
                    host = Arg.Remove(0, "-host=".Length);
                }                
            }


            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];
            rid[0].usUsagePage = (ushort)SharpLib.Hid.UsagePage.Telephony;
            rid[0].usUsage = (ushort)SharpLib.Hid.UsageCollection.Telephony.Headset;
            rid[0].dwFlags = (RawInputDeviceFlags)256;
            rid[0].hwndTarget = Handle;
            iHidHandler = new SharpLib.Hid.Handler(rid, false, -1, -1);
            iHidParser = iHidHandler;
            iHidParser.OnHidEvent += HandleHidEventThreadSafe;

            //notifyIcon1.Icon = new System.Drawing.Icon(System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal) + @"\Icon.ico");
            components = new System.ComponentModel.Container();
            notifyIcon1 = new System.Windows.Forms.NotifyIcon(components);
            notifyIcon1.Icon = new System.Drawing.Icon("App.ico");
            notifyIcon1.Visible = false;
            notifyIcon1.Text = "Incoming call";

        }
        public void HandleHidEventThreadSafe(object aSender, SharpLib.Hid.Event aHidEvent)
        {
            if (aHidEvent.IsStray)
            {
                //Stray event just ignore it
                return;
            }

            if (this.InvokeRequired)
            {
                //Not in the proper thread, invoke ourselves.
                //Repeat events usually come from another thread.
                OnHidEventDelegate d = new OnHidEventDelegate(HandleHidEventThreadSafe);
                this.Invoke(d, new object[] { aSender, aHidEvent });
            }
            else
            {
                //We are in the proper thread
                //listViewEvents.Items.Insert(0, aHidEvent.ToListViewItem());
                if (aHidEvent.Device != null)
                {
                    //toolStripStatusLabelDevice.Text = aHidEvent.Device.FriendlyName;
                }
                //Debug.Write(aHidEvent.ToLog());

                if (aHidEvent.IsButtonDown)
                {
                    notifyIcon1.Visible = !notifyIcon1.Visible;
                    string ST = CallApi("GET", "astproxy/extension/" + Ext, "");
                    dynamic data = JObject.Parse(ST);                    
                    string Status = (string)data.status;
                    string Ans, Hang;
                    switch (Status)
                    {
                        case "ringing":
                            if (Type == "webrtc") {
                                Ans = CallApi("POST", "astproxy/answer_webrtc", "{\"endpointId\":\"" + Ext + "\",\"endpointType\":\"extension\"}");
                            }
                            else {
                                Ans = CallApi("POST", "astproxy/answer", "{\"endpointId\":\"" + Ext + "\",\"endpointType\":\"extension\"}");
                            }
                            break;
                        case "busy":
                            string Conv = "";
                            foreach (JProperty Convx in data.conversations.Properties())
                            {
                                Conv=(string)Convx.Name.ToString();
                            }
                            Hang = CallApi("POST", "astproxy/hangup", "{\"convid\":\""+Conv+"\",\"endpointId\":\""+Ext+"\"}");
                            break;
                        case "online":
                            break;
                        case "offline":
                            break;
                    }
                }
            }

        }
        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                //case Const.WM_KEYDOWN:
                //ProcessKeyDown(message.WParam);
                //break;
                case Const.WM_INPUT:
                    //Log that message
                    //Debug.Write("WM_INPUT: " + message.ToString() + "\r\n");
                    //Returning zero means we processed that message.
                    message.Result = new IntPtr(0);
                    //iHidHandler.ProcessInput(ref message);
                    iHidParser.ProcessInput(ref message);
                    break;
            }
            //Is that needed? Check the docs.
            base.WndProc(ref message);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Token=GetToken();
            
            string ME=CallApi("GET","user/me","");
            dynamic data = JObject.Parse(ME);
            Ext=(string)data.default_device.id;
            Type=(string)data.default_device.type;
        }
        private string CallApi(string Method,string API, string payload)
        {
            string Url = "https://" + host + "/webrest/"+API;
            string data = "";
            WebRequest s = WebRequest.Create(Url);
            s.Method = Method;
            s.Headers.Add("Authorization", username + ":" + Token);
            if ((payload.Length>0)) 
            { 
                s.ContentType = "application/json";
                using (StreamWriter SW = new StreamWriter(s.GetRequestStream()))
                {
                    SW.Write(payload);
                }
            }
            WebResponse R=s.GetResponse();
            using (Stream dataStream = R.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                data = reader.ReadToEnd();
            }
            R.Close();
            return WebUtility.HtmlDecode(data);
        }

        private string GetToken() 
        {
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf CertificateHandler;
            string Nonce="";
            string postdata;
            string Url = "https://" + host + "/webrest/authentication/login";
            WebRequest s = WebRequest.Create(Url);
            s.Method = "POST";
            postdata = "username=" + WebUtility.UrlEncode(username) + "&password=" + WebUtility.UrlEncode(password);
            Byte[] postdatabytes = Encoding.UTF8.GetBytes(postdata);

            //'s.Timeout = 30
            s.ContentType = "application/x-www-form-urlencoded";
            s.ContentLength = postdatabytes.Length;

            using (var Stream = s.GetRequestStream())
            {
                Stream.Write(postdatabytes, 0, postdatabytes.Length);
            }

            // 'Token
            try
            {
                Nonce = s.GetResponse().Headers.GetValues("www-authenticate").Single().Replace("Digest ", "").Trim();
                return Nonce;
            }
            catch (Exception ex)
            {
                bool DoExit = true;
                                if (ex is WebException)
                                {
                                    WebException wEx = (WebException)ex;
                                    if (wEx.Response != null)
                                    {

                                        // can use ex.Response.Status, .StatusDescription

                                        HttpWebResponse Resp = (HttpWebResponse)wEx.Response;
                                        if (Resp.StatusCode == HttpStatusCode.Unauthorized)
                                        {
                                            Nonce = wEx.Response.Headers.GetValues("www-authenticate").Single().Replace("Digest ", "").Trim();
                                            DoExit = false;
                                        }
                                        else

                                            // Forse sta riavviando il server
                                            if (Resp.StatusCode == HttpStatusCode.ServiceUnavailable )
                                        {
                                            // Retry connection for 5 times???
                                            Application.DoEvents();
                                            System.Threading.Thread.Sleep(2000);
                                            Application.DoEvents();
                                        }
                                        else
                                        {                
                                            Debug.Write(new Exception(Resp.StatusDescription + " (" + Resp.StatusCode + ")"));
                                           
                                            //if (IsAutoConnecting)
                                            //{
                                            //    // Auto connect
                                            //    if (TryConnectionCount <= 5)
                                            //        ActivateReconnection();
                                            //    else
                                            //        InteruptReconnection();
                                            //}
                                        }
                                    }
                                }
                                
                                if (DoExit)
                                    Nonce = "";
                
                                var enc = Encoding.ASCII;
                                HMACSHA1 hmac = new HMACSHA1(enc.GetBytes(password));
                                hmac.Initialize();

                                byte[] buffer = enc.GetBytes(username + ":" + password + ":" + Nonce);
                
                return BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "").ToLower();
            }
        }
    }
}
