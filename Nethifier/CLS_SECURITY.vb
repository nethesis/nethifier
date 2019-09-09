Imports System.IO
Imports System.Text
Imports System.Security.Cryptography

Friend Class CLS_SECURITY

    Friend Class Aes256Base64Encrypter
        Public Function Encrypt(ByVal plainText As String, ByVal secretKey As String, Optional DoScramble As Boolean = False) As String
            Dim encryptedPassword As String = Nothing
            Using outputStream As MemoryStream = New MemoryStream()
                Dim algorithm As RijndaelManaged = getAlgorithm(secretKey, DoScramble)
                Using cryptoStream As CryptoStream = New CryptoStream(outputStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write)
                    Dim inputBuffer() As Byte = Encoding.Unicode.GetBytes(plainText)
                    cryptoStream.Write(inputBuffer, 0, inputBuffer.Length)
                    cryptoStream.FlushFinalBlock()
                    encryptedPassword = Convert.ToBase64String(outputStream.ToArray())

                    If DoScramble Then
                        encryptedPassword = SecurityHelper.FurtherScramble(encryptedPassword)
                    End If

                End Using
            End Using
            Return encryptedPassword
        End Function

        Public Function Decrypt(ByVal encryptedBytes As String, ByVal secretKey As String, Optional DoScramble As Boolean = False) As String
            Dim plainText As String = Nothing
            Try
                If DoScramble Then
                    encryptedBytes = SecurityHelper.FurtherScramble(encryptedBytes)
                End If

                Using inputStream As MemoryStream = New MemoryStream(Convert.FromBase64String(encryptedBytes))
                    Dim algorithm As RijndaelManaged = getAlgorithm(secretKey, DoScramble)
                    Using cryptoStream As CryptoStream = New CryptoStream(inputStream, algorithm.CreateDecryptor(), CryptoStreamMode.Read)
                        Dim outputBuffer(0 To CType(inputStream.Length - 1, Integer)) As Byte
                        Dim readBytes As Integer = cryptoStream.Read(outputBuffer, 0, CType(inputStream.Length, Integer))
                        plainText = Encoding.Unicode.GetString(outputBuffer, 0, readBytes)
                    End Using
                End Using
            Catch ex As Exception
                plainText = ex.Message
            End Try
            Return plainText
        End Function

        Private Function getAlgorithm(ByVal secretKey As String, Optional DoScramble As Boolean = False) As RijndaelManaged
            Const salt As String = "put your salt here"
            Const keySize As Integer = 256

            If DoScramble Then
                secretKey = SecurityHelper.FurtherScramble(secretKey)
            End If

            Dim keyBuilder As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(secretKey, Encoding.Unicode.GetBytes(salt))
            Dim algorithm As RijndaelManaged = New RijndaelManaged()
            algorithm.KeySize = keySize
            algorithm.IV = keyBuilder.GetBytes(CType(algorithm.BlockSize / 8, Integer))
            algorithm.Key = keyBuilder.GetBytes(CType(algorithm.KeySize / 8, Integer))
            algorithm.Padding = PaddingMode.PKCS7
            Return algorithm
        End Function
    End Class

    Protected Friend Class SecurityHelper
        Protected Friend Shared Function FurtherScramble(Key As String) As String
            Dim BLOCK_A As String = "ABCDEFGHIJKLM"
            Dim BLOCK_A_small As String = "abcdefghijklm"
            Dim BLOCK_B As String = "NOPQRSTUVWXYZ"
            Dim BLOCK_B_small As String = "nopqrstuvwxyz"

            Dim BLOCK_N0 As String = "01234"
            Dim BLOCK_N1 As String = "56789"

            Dim NewKey As String = ""

            Try
                For I = 0 To Key.Length - 1
                    Dim K As String = Key.Substring(I, 1)

                    If IsNumeric(K) Then
                        NewKey += FindPosition(K, BLOCK_N0, BLOCK_N1)
                    Else
                        If BLOCK_A.ToLower.IndexOf(K.ToLower) > -1 OrElse BLOCK_B.ToLower.IndexOf(K.ToLower) > -1 Then
                            If IsUpperCase(K) Then
                                NewKey += FindPosition(K, BLOCK_A, BLOCK_B)
                            Else
                                NewKey += FindPosition(K, BLOCK_A_small, BLOCK_B_small)
                            End If
                        Else
                            NewKey += K
                        End If
                    End If
                Next
            Catch ex As Exception
                Dim X As Exception = ex
            End Try

            Return NewKey
        End Function

        Protected Friend Shared Function IsUpperCase(L As String) As Boolean
            Return L = L.ToUpper
        End Function

        Protected Friend Shared Function FindPosition(K As String, BLOCK0 As String, BLOCK1 As String) As String
            Dim NX As Integer = BLOCK0.IndexOf(K)
            If NX > -1 Then
                Return BLOCK1.Substring(NX, 1)
            End If

            NX = BLOCK1.IndexOf(K)
            If NX > -1 Then
                Return BLOCK0.Substring(NX, 1)
            End If

            Return K
        End Function
    End Class
End Class
