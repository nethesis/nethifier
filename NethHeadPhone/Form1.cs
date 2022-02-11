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
using System.Threading;

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
        public int retry = 1;

        string Ext;
        string Type;

        public HeadPhone()
        {
            InitializeComponent();
            Trace.WriteLine("Initialized");

            foreach (string Arg in Environment.GetCommandLineArgs()) {
                if (Arg.ToLower().StartsWith("-username=")) {
                    username = Arg.Remove(0, "-username=".Length);
                } else if (Arg.ToLower().StartsWith("-password=")) { 
                    password = Arg.Remove(0, "-password=".Length);
                } else if (Arg.ToLower().StartsWith("-host=")) {
                    host = Arg.Remove(0, "-host=".Length);
                }                
            }

            if (host.Length==0) {
                MessageBox.Show("HeadPhone not running");
                System.Environment.Exit(1);
            }

            Trace.WriteLine("Read Parms..");

            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];
            rid[0].usUsagePage = (ushort)SharpLib.Hid.UsagePage.Telephony;
            rid[0].usUsage = (ushort)SharpLib.Hid.UsageCollection.Telephony.Headset;
            rid[0].dwFlags = (RawInputDeviceFlags)256;
            rid[0].hwndTarget = Handle;
            Trace.WriteLine("Rid defined..");
            iHidHandler = new SharpLib.Hid.Handler(rid, false, -1, -1);
            iHidParser = iHidHandler;
            iHidParser.OnHidEvent += HandleHidEventThreadSafe;
            Trace.WriteLine("Handle Hid");

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
                Trace.WriteLine(aHidEvent.ToLog().Replace(Environment.NewLine, ""));
               
                if (((aHidEvent.IsButtonDown) & (aHidEvent.ToString().Contains("HookSwitch (0x0020)") | aHidEvent.ToString().Contains("LineBusyTone (0x0097)"))))
                {
                    string ST = CallApi("GET", "astproxy/extension/" + Ext, "");
                    dynamic data = JObject.Parse(ST);
                    string Status = (string)data.status;
                    Trace.WriteLine("Button click: " + Status + " with " + Ext.ToString());
                    string Ans, Hang;
                    switch (Status)
                    {
                        case "ringing":
                            if (Type == "webrtc")
                            {
                                Ans = CallApi("POST", "astproxy/answer_webrtc", "{\"endpointId\":\"" + Ext + "\",\"endpointType\":\"extension\"}");
                            }
                            else
                            {
                                Ans = CallApi("POST", "astproxy/answer", "{\"endpointId\":\"" + Ext + "\",\"endpointType\":\"extension\"}");
                            }
                            break;
                        case "busy":
                            string Conv = "";
                            foreach (JProperty Convx in data.conversations.Properties())
                            {
                                Conv = (string)Convx.Name.ToString();
                            }
                            Hang = CallApi("POST", "astproxy/hangup", "{\"convid\":\"" + Conv + "\",\"endpointId\":\"" + Ext + "\"}");
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
                case Const.WM_INPUT:
                    message.Result = new IntPtr(0);
                    iHidParser.ProcessInput(ref message);
                    break;
            }
            //Is that needed? Check the docs.
            base.WndProc(ref message);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Token=GetToken();
            Trace.WriteLine("Got Token..");

            string ME=CallApi("GET","user/me","");
            Trace.WriteLine("Called API: me");
            dynamic data = JObject.Parse(ME);
            Ext=(string)data.default_device.id;
            Type=(string)data.default_device.type;
            Trace.WriteLine("Loaded..");
            Token = "";
        }
        private string CallApi(string Method,string API, string payload)
        {
            string Url = "https://" + host + "/webrest/"+API;
            string data = "";
            try
            {
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
                retry = 1;
                return WebUtility.HtmlDecode(data);
            }
            catch
            {
                Thread.Sleep(10000*retry);
                retry=retry*2;
                if (retry > 32)
                    {
                        MessageBox.Show("Per l'utilizzo della cuffia è necessario rilanciare Nethifier.");
                        System.Environment.Exit(1);
                    }
                Token = GetToken();
                return CallApi(Method, API, payload); 
            }
            
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
                Trace.Write("Exception caught: "+ex.Message.ToString());
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
                                            // Maybe the server is restarting
                                            if (Resp.StatusCode == HttpStatusCode.ServiceUnavailable )
                                        {
                                            // Retry connection for 5 times???
                                            Trace.Write("Waiting: Service unavailable");
                                            Application.DoEvents();
                                            System.Threading.Thread.Sleep(2000);
                                            Application.DoEvents();
                                        }
                                        else
                                        {                
                                            Trace.Write(new Exception(Resp.StatusDescription + " (" + Resp.StatusCode + ")"));                                           
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

