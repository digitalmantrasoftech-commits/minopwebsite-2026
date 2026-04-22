
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using MasterData.Models;
using System.Security.Claims;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Collections;
using System.Threading;
namespace PayTimeWebClient.Helper
{
    //public class NHibernateHelper
    //{

    //    #region Master DataBase
    //    private static string dbMasterCon = ConfigurationManager.AppSettings["paytimewebMasterConStr"].ToString();
    //    private static ISessionFactory _mastersessionFactory;
    //    private static string MasterconnectionString = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["paytimewebMasterConStr"].ToString()].ToString();  //"Data Source=192.168.1.127;Initial Catalog=PaytimeWeb;Persist Security Info=True;User ID=sa;Password=Mantra@123";
    //    public static ISession OpenSession(bool IsMaster)
    //    {
    //        return MasterSessionFactory.OpenSession();
    //    }
    //    private static ISessionFactory MasterSessionFactory
    //    {
    //        get
    //        {
    //            if (_mastersessionFactory == null)
    //                CreateSessionMasterFactory();

    //            return _mastersessionFactory;
    //        }
    //    }
    //    private static void CreateSessionMasterFactory()
    //    {
    //        if (dbMasterCon == "MASSQL")
    //        {

    //            _mastersessionFactory = Fluently.Configure()
    //                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(MasterconnectionString).ShowSql)
    //                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<RegisteredCompany>())
    //               .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false))
    //            .BuildSessionFactory();
    //        }
    //        if (dbMasterCon == "MASMySql")
    //        {
    //            _mastersessionFactory = Fluently.Configure()
    //                .Database(MySQLConfiguration.Standard.ConnectionString(MasterconnectionString).ShowSql)
    //                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<RegisteredCompany>())
    //               .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false))
    //            .BuildSessionFactory();
    //        }
    //        if (dbMasterCon == "MASOracle")
    //        {
    //            _mastersessionFactory = Fluently.Configure()
    //                .Database(OracleClientConfiguration.Oracle10.ConnectionString(MasterconnectionString).ShowSql)
    //                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<RegisteredCompany>())
    //               .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false))
    //            .BuildSessionFactory();
    //        }
    //    }
    //    #endregion

    //    #region Client Database
    //    public static ISession OpenSession()
    //    {
    //        return SessionFactory.OpenSession();
    //    }



    //    public static string dbCon = ConfigurationManager.AppSettings["paytimewebConStr"].ToString();
    //    private static ISessionFactory _sessionFactory;
    //    public static string connectionString = "";
    //    public static int curUid = 0;
    //    public static string CompCode = "";
    //    public static string strcn = "";
    //    // ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["paytimewebConStr"].ToString()].ToString();  //"Data Source=192.168.1.127;Initial Catalog=PaytimeWeb;Persist Security Info=True;User ID=sa;Password=Mantra@123";
    //    private static ISessionFactory SessionFactory
    //    {

    //        get
    //        {
    //            //_sessionFactory = null;
    //            if (_sessionFactory == null)
    //                CreateSessionFactory();
    //            return _sessionFactory;
    //        }
    //    }
    //    private static void CreateSessionFactory()
    //    {
    //        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

    //        // Get the claims values
    //        //var xcon = identity.Claims.Where(c => c.Type == ClaimTypes.StreetAddress)
    //        //                   .Select(c => c.Value).SingleOrDefault();
    //        //var cuid = identity.Claims.Where(c => c.Type == ClaimTypes.Surname)
    //        //                    .Select(c => c.Value).SingleOrDefault();
    //        var xcon = identity.Claims.Where(c => c.Type == "keystr")
    //                         .Select(c => c.Value).SingleOrDefault();
    //        var cuid = identity.Claims.Where(c => c.Type == "UserId")
    //                            .Select(c => c.Value).SingleOrDefault();


    //        if (cuid != null)
    //        {
    //            curUid = Convert.ToInt32(cuid);
    //        }
    //        if (xcon != null)
    //        {
    //            var xstrcon = xcon.Split('|')[0];
    //            var xccode = xcon.Split('|')[1];
    //            dbCon = xcon.Split('|')[2];
    //            CompCode = xccode.ToString();
    //            connectionString = EncryptionHelper.Decrypt(xstrcon);
    //        }
    //        //dbCon = dbMasterCon.ToUpper() == "MASSQL" ? "SQL" : dbMasterCon.ToUpper() == "MASMYSQL" ? "MYSQL" : "ORACLE";
    //        if (dbCon.ToUpper() == "SQL")
    //        {

    //            _sessionFactory = Fluently.Configure()
    //                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connectionString).ShowSql)
    //                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Registration>())
    //                //.Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<DeviceUsers>()))
    //               .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false))
    //            .BuildSessionFactory();

    //        }
    //        if (dbCon.ToUpper() == "MYSQL")
    //        {
    //            _sessionFactory = Fluently.Configure()
    //                .Database(MySQLConfiguration.Standard.ConnectionString(connectionString).ShowSql)
    //                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Registration>())
    //                //.Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<DeviceUsers>()))
    //               .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false))
    //            .BuildSessionFactory();
    //        }
    //        if (dbCon.ToUpper() == "ORACLE")
    //        {
    //            _sessionFactory = Fluently.Configure()
    //                .Database(OracleClientConfiguration.Oracle10.ConnectionString(connectionString).ShowSql)
    //                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Registration>())
    //               .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false))
    //            .BuildSessionFactory();
    //        }
    //    }
    //    #endregion
    //}

    public class EncryptionHelper
    {
        public string Encrypt(string clearText)
        {
            string EncryptionKey = ConfigurationManager.AppSettings.Get("jwtKey");
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = ConfigurationManager.AppSettings.Get("jwtKey");
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
   
    public class Authorization
    {
        public byte[] GetBytes(string input)
        {
            var bytes = new byte[input.Length * sizeof(char)];
            Buffer.BlockCopy(input.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;

        }
    }

    public class RijndaelCrypt
    {

        
        #region Private/Protected Member Variables

        /// <summary>
        /// Decryptor
        /// 
        private readonly ICryptoTransform _decryptor;

        /// <summary>
        /// Encryptor
        /// 
        private readonly ICryptoTransform _encryptor;

        /// <summary>
        /// 16-byte Private Key
        /// 
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("Its%v9VV8W][uY!s");

        /// <summary>
        /// Public Key
        /// 
        private readonly byte[] _password;

        /// <summary>
        /// Rijndael cipher algorithm
        /// 
        private readonly RijndaelManaged _cipher;

        #endregion

        #region Private/Protected Properties

        private ICryptoTransform Decryptor { get { return _decryptor; } }
        private ICryptoTransform Encryptor { get { return _encryptor; } }

        #endregion

        #region Private/Protected Methods
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// 
        /// <param name="password">Public key
        public RijndaelCrypt(string password)
        {
            //Encode digest
            var md5 = new MD5CryptoServiceProvider();
            _password = md5.ComputeHash(Encoding.ASCII.GetBytes(password));

            //Initialize objects
            _cipher = new RijndaelManaged();
            _decryptor = _cipher.CreateDecryptor(_password, IV);
            _encryptor = _cipher.CreateEncryptor(_password, IV);

        }

        #endregion

        #region Public Properties
        #endregion

        #region Public Methods

        /// <summary>
        /// Decryptor
        /// 
        /// <param name="text">Base64 string to be decrypted
        /// <returns>
        public string Decrypt(string text)
        {
            try
            {
                byte[] input = Convert.FromBase64String(text);

                var newClearData = Decryptor.TransformFinalBlock(input, 0, input.Length);
                return Encoding.ASCII.GetString(newClearData);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("inputCount uses an invalid value or inputBuffer has an invalid offset length. " + ae);
                return null;
            }
            catch (ObjectDisposedException oe)
            {
                Console.WriteLine("The object has already been disposed." + oe);
                return null;
            }


        }



        /// <summary>
        /// Encryptor
        /// 
        /// <param name="text">String to be encrypted 
        /// <returns>
        public string Encrypt(string text)
        {
            try
            {
                var buffer = Encoding.ASCII.GetBytes(text);
                var j =Encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                string abc= Convert.ToBase64String(Encryptor.TransformFinalBlock(buffer, 0, buffer.Length));
                return abc;
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("inputCount uses an invalid value or inputBuffer has an invalid offset length. " + ae);
                return null;
            }
            catch (ObjectDisposedException oe)
            {
                Console.WriteLine("The object has already been disposed." + oe);
                return null;
            }

        }

        #endregion
    }


}