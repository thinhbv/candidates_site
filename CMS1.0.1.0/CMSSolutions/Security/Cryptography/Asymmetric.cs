using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CMSSolutions.Security.Cryptography
{
    /// <summary>
    /// Asymmetric encryption uses a pair of keys to encrypt and decrypt.
    /// There is a "public" key which is used to encrypt. Decrypting, on the other hand,
    /// requires both the "public" key and an additional "private" key. The advantage is
    /// that people can send you encrypted messages without being able to decrypt them.
    /// </summary>
    /// <remarks>
    /// The only provider supported is the <see cref="RSACryptoServiceProvider"/>
    /// </remarks>
    public class Asymmetric
    {
        private RSACryptoServiceProvider _rsa;
        private string _KeyContainerName = "Encryption.AsymmetricEncryption.DefaultContainerName";
        private int _KeySize = 1024;

        private const string _ElementParent = "RSAKeyValue";
        private const string _ElementModulus = "Modulus";
        private const string _ElementExponent = "Exponent";
        private const string _ElementPrimeP = "P";
        private const string _ElementPrimeQ = "Q";
        private const string _ElementPrimeExponentP = "DP";
        private const string _ElementPrimeExponentQ = "DQ";
        private const string _ElementCoefficient = "InverseQ";
        private const string _ElementPrivateExponent = "D";

        //-- http://forum.java.sun.com/thread.jsp?forum=9&thread=552022&tstart=0&trange=15
        private const string _KeyModulus = "PublicKey.Modulus";

        private const string _KeyExponent = "PublicKey.Exponent";
        private const string _KeyPrimeP = "PrivateKey.P";
        private const string _KeyPrimeQ = "PrivateKey.Q";
        private const string _KeyPrimeExponentP = "PrivateKey.DP";
        private const string _KeyPrimeExponentQ = "PrivateKey.DQ";
        private const string _KeyCoefficient = "PrivateKey.InverseQ";
        private const string _KeyPrivateExponent = "PrivateKey.D";

        #region PublicKey Class

        /// <summary>
        /// Represents a public encryption key. Intended to be shared, it
        /// contains only the Modulus and Exponent.
        /// </summary>
        public class PublicKey
        {
            public string Modulus;
            public string Exponent;

            public PublicKey()
            {
            }

            public PublicKey(string KeyXml)
            {
                LoadFromXml(KeyXml);
            }

            /// <summary>
            /// Load public key from App.config or Web.config file
            /// </summary>
            public void LoadFromConfig()
            {
                this.Modulus = EncryptionExtensions.GetConfigString(_KeyModulus);
                this.Exponent = EncryptionExtensions.GetConfigString(_KeyExponent);
            }

            /// <summary>
            /// Returns *.config file XML section representing this public key
            /// </summary>
            public string ToConfigSection()
            {
                StringBuilder sb = new StringBuilder();
                {
                    sb.Append(EncryptionExtensions.WriteConfigKey(_KeyModulus, this.Modulus));
                    sb.Append(EncryptionExtensions.WriteConfigKey(_KeyExponent, this.Exponent));
                }
                return sb.ToString();
            }

            /// <summary>
            /// Writes the *.config file representation of this public key to a file
            /// </summary>
            public void ExportToConfigFile(string filePath)
            {
                StreamWriter sw = new StreamWriter(filePath, false);
                sw.Write(this.ToConfigSection());
                sw.Close();
            }

            /// <summary>
            /// Loads the public key from its XML string
            /// </summary>
            public void LoadFromXml(string keyXml)
            {
                this.Modulus = EncryptionExtensions.GetXmlElement(keyXml, "Modulus");
                this.Exponent = EncryptionExtensions.GetXmlElement(keyXml, "Exponent");
            }

            /// <summary>
            /// Converts this public key to an RSAParameters object
            /// </summary>
            public RSAParameters ToParameters()
            {
                RSAParameters r = new RSAParameters();
                r.Modulus = Convert.FromBase64String(this.Modulus);
                r.Exponent = Convert.FromBase64String(this.Exponent);
                return r;
            }

            /// <summary>
            /// Converts this public key to its XML string representation
            /// </summary>
            public string ToXml()
            {
                StringBuilder sb = new StringBuilder();
                {
                    sb.Append(EncryptionExtensions.WriteXmlNode(_ElementParent));
                    sb.Append(EncryptionExtensions.WriteXmlElement(_ElementModulus, this.Modulus));
                    sb.Append(EncryptionExtensions.WriteXmlElement(_ElementExponent, this.Exponent));
                    sb.Append(EncryptionExtensions.WriteXmlNode(_ElementParent, true));
                }
                return sb.ToString();
            }

            /// <summary>
            /// Writes the Xml representation of this public key to a file
            /// </summary>
            public void ExportToXmlFile(string filePath)
            {
                StreamWriter sw = new StreamWriter(filePath, false);
                sw.Write(this.ToXml());
                sw.Close();
            }
        }

        #endregion PublicKey Class

        #region "  PrivateKey Class"

        /// <summary>
        /// Represents a private encryption key. Not intended to be shared, as it
        /// contains all the elements that make up the key.
        /// </summary>
        public class PrivateKey
        {
            public string Modulus;
            public string Exponent;
            public string PrimeP;
            public string PrimeQ;
            public string PrimeExponentP;
            public string PrimeExponentQ;
            public string Coefficient;
            public string PrivateExponent;

            public PrivateKey()
            {
            }

            public PrivateKey(string keyXml)
            {
                LoadFromXml(keyXml);
            }

            /// <summary>
            /// Load private key from App.config or Web.config file
            /// </summary>
            public void LoadFromConfig()
            {
                this.Modulus = EncryptionExtensions.GetConfigString(_KeyModulus);
                this.Exponent = EncryptionExtensions.GetConfigString(_KeyExponent);
                this.PrimeP = EncryptionExtensions.GetConfigString(_KeyPrimeP);
                this.PrimeQ = EncryptionExtensions.GetConfigString(_KeyPrimeQ);
                this.PrimeExponentP = EncryptionExtensions.GetConfigString(_KeyPrimeExponentP);
                this.PrimeExponentQ = EncryptionExtensions.GetConfigString(_KeyPrimeExponentQ);
                this.Coefficient = EncryptionExtensions.GetConfigString(_KeyCoefficient);
                this.PrivateExponent = EncryptionExtensions.GetConfigString(_KeyPrivateExponent);
            }

            /// <summary>
            /// Converts this private key to an RSAParameters object
            /// </summary>
            public RSAParameters ToParameters()
            {
                RSAParameters r = new RSAParameters();
                r.Modulus = Convert.FromBase64String(this.Modulus);
                r.Exponent = Convert.FromBase64String(this.Exponent);
                r.P = Convert.FromBase64String(this.PrimeP);
                r.Q = Convert.FromBase64String(this.PrimeQ);
                r.DP = Convert.FromBase64String(this.PrimeExponentP);
                r.DQ = Convert.FromBase64String(this.PrimeExponentQ);
                r.InverseQ = Convert.FromBase64String(this.Coefficient);
                r.D = Convert.FromBase64String(this.PrivateExponent);
                return r;
            }

            /// <summary>
            /// Returns *.config file XML section representing this private key
            /// </summary>
            public string ToConfigSection()
            {
                StringBuilder sb = new StringBuilder();
                {
                    sb.Append(EncryptionExtensions.WriteConfigKey(_KeyModulus, this.Modulus));
                    sb.Append(EncryptionExtensions.WriteConfigKey(_KeyExponent, this.Exponent));
                    sb.Append(EncryptionExtensions.WriteConfigKey(_KeyPrimeP, this.PrimeP));
                    sb.Append(EncryptionExtensions.WriteConfigKey(_KeyPrimeQ, this.PrimeQ));
                    sb.Append(EncryptionExtensions.WriteConfigKey(_KeyPrimeExponentP, this.PrimeExponentP));
                    sb.Append(EncryptionExtensions.WriteConfigKey(_KeyPrimeExponentQ, this.PrimeExponentQ));
                    sb.Append(EncryptionExtensions.WriteConfigKey(_KeyCoefficient, this.Coefficient));
                    sb.Append(EncryptionExtensions.WriteConfigKey(_KeyPrivateExponent, this.PrivateExponent));
                }
                return sb.ToString();
            }

            /// <summary>
            /// Writes the *.config file representation of this private key to a file
            /// </summary>
            public void ExportToConfigFile(string strFilePath)
            {
                StreamWriter sw = new StreamWriter(strFilePath, false);
                sw.Write(this.ToConfigSection());
                sw.Close();
            }

            /// <summary>
            /// Loads the private key from its XML string
            /// </summary>
            public void LoadFromXml(string keyXml)
            {
                this.Modulus = EncryptionExtensions.GetXmlElement(keyXml, "Modulus");
                this.Exponent = EncryptionExtensions.GetXmlElement(keyXml, "Exponent");
                this.PrimeP = EncryptionExtensions.GetXmlElement(keyXml, "P");
                this.PrimeQ = EncryptionExtensions.GetXmlElement(keyXml, "Q");
                this.PrimeExponentP = EncryptionExtensions.GetXmlElement(keyXml, "DP");
                this.PrimeExponentQ = EncryptionExtensions.GetXmlElement(keyXml, "DQ");
                this.Coefficient = EncryptionExtensions.GetXmlElement(keyXml, "InverseQ");
                this.PrivateExponent = EncryptionExtensions.GetXmlElement(keyXml, "D");
            }

            /// <summary>
            /// Converts this private key to its XML string representation
            /// </summary>
            public string ToXml()
            {
                StringBuilder sb = new StringBuilder();
                {
                    sb.Append(EncryptionExtensions.WriteXmlNode(_ElementParent));
                    sb.Append(EncryptionExtensions.WriteXmlElement(_ElementModulus, this.Modulus));
                    sb.Append(EncryptionExtensions.WriteXmlElement(_ElementExponent, this.Exponent));
                    sb.Append(EncryptionExtensions.WriteXmlElement(_ElementPrimeP, this.PrimeP));
                    sb.Append(EncryptionExtensions.WriteXmlElement(_ElementPrimeQ, this.PrimeQ));
                    sb.Append(EncryptionExtensions.WriteXmlElement(_ElementPrimeExponentP, this.PrimeExponentP));
                    sb.Append(EncryptionExtensions.WriteXmlElement(_ElementPrimeExponentQ, this.PrimeExponentQ));
                    sb.Append(EncryptionExtensions.WriteXmlElement(_ElementCoefficient, this.Coefficient));
                    sb.Append(EncryptionExtensions.WriteXmlElement(_ElementPrivateExponent, this.PrivateExponent));
                    sb.Append(EncryptionExtensions.WriteXmlNode(_ElementParent, true));
                }
                return sb.ToString();
            }

            /// <summary>
            /// Writes the Xml representation of this private key to a file
            /// </summary>
            public void ExportToXmlFile(string filePath)
            {
                StreamWriter sw = new StreamWriter(filePath, false);
                sw.Write(this.ToXml());
                sw.Close();
            }
        }

        #endregion "  PrivateKey Class"

        /// <summary>
        /// Instantiates a new asymmetric encryption session using the default key size;
        /// this is usally 1024 bits
        /// </summary>
        public Asymmetric()
        {
            _rsa = GetRSAProvider();
        }

        /// <summary>
        /// Instantiates a new asymmetric encryption session using a specific key size
        /// </summary>
        public Asymmetric(int keySize)
        {
            _KeySize = keySize;
            _rsa = GetRSAProvider();
        }

        /// <summary>
        /// Sets the name of the key container used to store this key on disk; this is an
        /// unavoidable side effect of the underlying Microsoft CryptoAPI.
        /// </summary>
        /// <remarks>
        /// http://support.microsoft.com/default.aspx?scid=http://support.microsoft.com:80/support/kb/articles/q322/3/71.asp&amp;NoWebContent=1
        /// </remarks>
        public string KeyContainerName
        {
            get { return _KeyContainerName; }
            set { _KeyContainerName = value; }
        }

        /// <summary>
        /// Returns the current key size, in bits
        /// </summary>
        public int KeySizeBits
        {
            get { return _rsa.KeySize; }
        }

        /// <summary>
        /// Returns the maximum supported key size, in bits
        /// </summary>
        public int KeySizeMaxBits
        {
            get { return _rsa.LegalKeySizes[0].MaxSize; }
        }

        /// <summary>
        /// Returns the minimum supported key size, in bits
        /// </summary>
        public int KeySizeMinBits
        {
            get { return _rsa.LegalKeySizes[0].MinSize; }
        }

        /// <summary>
        /// Returns valid key step sizes, in bits
        /// </summary>
        public int KeySizeStepBits
        {
            get { return _rsa.LegalKeySizes[0].SkipSize; }
        }

        /// <summary>
        /// Returns the default public key as stored in the *.config file
        /// </summary>
        public PublicKey DefaultPublicKey
        {
            get
            {
                PublicKey pubkey = new PublicKey();
                pubkey.LoadFromConfig();
                return pubkey;
            }
        }

        /// <summary>
        /// Returns the default private key as stored in the *.config file
        /// </summary>
        public PrivateKey DefaultPrivateKey
        {
            get
            {
                PrivateKey privkey = new PrivateKey();
                privkey.LoadFromConfig();
                return privkey;
            }
        }

        /// <summary>
        /// Generates a new public/private key pair as objects
        /// </summary>
        public void GenerateNewKeyset(ref PublicKey publicKey, ref PrivateKey privateKey)
        {
            string PublicKeyXML = null;
            string PrivateKeyXML = null;
            GenerateNewKeyset(ref PublicKeyXML, ref PrivateKeyXML);
            publicKey = new PublicKey(PublicKeyXML);
            privateKey = new PrivateKey(PrivateKeyXML);
        }

        /// <summary>
        /// Generates a new public/private key pair as XML strings
        /// </summary>
        public void GenerateNewKeyset(ref string publicKeyXML, ref string privateKeyXML)
        {
            RSA rsa = RSACryptoServiceProvider.Create();
            publicKeyXML = rsa.ToXmlString(false);
            privateKeyXML = rsa.ToXmlString(true);
        }

        /// <summary>
        /// Encrypts data using the default public key
        /// </summary>
        public Data Encrypt(Data d)
        {
            PublicKey PublicKey = DefaultPublicKey;
            return Encrypt(d, PublicKey);
        }

        /// <summary>
        /// Encrypts data using the provided public key
        /// </summary>
        public Data Encrypt(Data d, PublicKey publicKey)
        {
            _rsa.ImportParameters(publicKey.ToParameters());
            return EncryptPrivate(d);
        }

        /// <summary>
        /// Encrypts data using the provided public key as XML
        /// </summary>
        public Data Encrypt(Data d, string publicKeyXML)
        {
            LoadKeyXml(publicKeyXML, false);
            return EncryptPrivate(d);
        }

        private Data EncryptPrivate(Data d)
        {
            try
            {
                return new Data(_rsa.Encrypt(d.Bytes, false));
            }
            catch (CryptographicException ex)
            {
                if (ex.Message.ToLower().IndexOf("bad length") > -1)
                {
                    throw new CryptographicException("Your data is too large; RSA encryption is designed to encrypt relatively small amounts of data. The exact byte limit depends on the key size. To encrypt more data, use symmetric encryption and then encrypt that symmetric key with asymmetric RSA encryption.", ex);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Decrypts data using the default private key
        /// </summary>
        public Data Decrypt(Data encryptedData)
        {
            PrivateKey PrivateKey = new PrivateKey();
            PrivateKey.LoadFromConfig();
            return Decrypt(encryptedData, PrivateKey);
        }

        /// <summary>
        /// Decrypts data using the provided private key
        /// </summary>
        public Data Decrypt(Data encryptedData, PrivateKey PrivateKey)
        {
            _rsa.ImportParameters(PrivateKey.ToParameters());
            return DecryptPrivate(encryptedData);
        }

        /// <summary>
        /// Decrypts data using the provided private key as XML
        /// </summary>
        public Data Decrypt(Data encryptedData, string PrivateKeyXML)
        {
            LoadKeyXml(PrivateKeyXML, true);
            return DecryptPrivate(encryptedData);
        }

        private void LoadKeyXml(string keyXml, bool isPrivate)
        {
            try
            {
                _rsa.FromXmlString(keyXml);
            }
            catch (System.Security.XmlSyntaxException ex)
            {
                string s = null;
                if (isPrivate)
                {
                    s = "private";
                }
                else
                {
                    s = "public";
                }
                throw new System.Security.XmlSyntaxException(string.Format("The provided {0} encryption key XML does not appear to be valid.", s), ex);
            }
        }

        private Data DecryptPrivate(Data encryptedData)
        {
            return new Data(_rsa.Decrypt(encryptedData.Bytes, false));
        }

        /// <summary>
        /// gets the default RSA provider using the specified key size;
        /// note that Microsoft's CryptoAPI has an underlying file system dependency that is unavoidable
        /// </summary>
        /// <remarks>
        /// http://support.microsoft.com/default.aspx?scid=http://support.microsoft.com:80/support/kb/articles/q322/3/71.asp&amp;NoWebContent=1
        /// </remarks>
        private RSACryptoServiceProvider GetRSAProvider()
        {
            RSACryptoServiceProvider rsa = null;
            CspParameters csp = null;
            try
            {
                csp = new CspParameters();
                csp.KeyContainerName = _KeyContainerName;
                rsa = new RSACryptoServiceProvider(_KeySize, csp);
                rsa.PersistKeyInCsp = false;
                RSACryptoServiceProvider.UseMachineKeyStore = true;
                return rsa;
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                if (ex.Message.ToLower().IndexOf("csp for this implementation could not be acquired") > -1)
                {
                    throw new Exception("Unable to obtain Cryptographic Service Provider. Either the permissions are incorrect on the 'C:\\Documents and Settings\\All Users\\Application Data\\Microsoft\\Crypto\\RSA\\MachineKeys' folder, or the current security context '" + System.Security.Principal.WindowsIdentity.GetCurrent().Name + "' does not have access to this folder.", ex);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if ((rsa != null))
                {
                    rsa = null;
                }
                if ((csp != null))
                {
                    csp = null;
                }
            }
        }
    }
}