using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

/// <summary>
/// Summary description for TXL
/// </summary>
public class TXL
{
    #region Variables
    private TXLDatabase database = null;
    private TXLFilteringPagingSorting fps = null;
    #endregion

    #region Properties
    public TXLDatabase Database
    {
        get
        {
            if (database == null)
                database = new TXLDatabase();
            return database;
        }
    }

    public TXLFilteringPagingSorting FPS
    {
        get
        {
            if (fps == null)
                fps = new TXLFilteringPagingSorting();
            return fps;
        }
    }
    #endregion

    #region Public methods
    public string GeneratePassword(int Length, IEnumerable<string> CharCollectionKeys)
    {
        string password = string.Empty;

        try
        {
            if (Length > 0)
            {
                // Create lists
                Dictionary<string, string> charCollections = new Dictionary<string, string>
            {
                { "AlphaLowerCase", "abcdefghjkmnpqrstuvwxyz" },
                { "AlphaUpperCase", "ABCDEFGHJKMNPQRSTUVWXYZ" },
                { "Numeric", "23456789" },
                { "Similar", "iloILO01" }
            };

                // Add char collection to list
                ArrayList list = new ArrayList();
                if (CharCollectionKeys != null)
                    foreach (string key in CharCollectionKeys)
                        if (charCollections.ContainsKey(key))
                            list.AddRange(charCollections[key].ToCharArray());

                if (list.Count > 0)
                {
                    // Get random chars
                    Random rand = new Random();
                    while (password.Length < Length)
                        password += list[rand.Next(list.Count - 1)].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            password = string.Empty;
        }

        return password;
    }
    #endregion
}

public class TXLDatabase
{
    #region Variables
    private string defaultConnectionName = "SiteSqlConnection";
    private Dictionary<string, SqlConnection> sqlConnections = null;
    #endregion

    #region Properties
    public string DefaultConnectionName
    {
        get
        {
            return defaultConnectionName;
        }
        set
        {
            defaultConnectionName = value;
        }
    }

    public Dictionary<string, SqlConnection> SqlConnections
    {
        get
        {
            if (sqlConnections == null)
            {
                sqlConnections = new Dictionary<string, SqlConnection>();
                for (int i = 0; i < WebConfigurationManager.ConnectionStrings.Count; i++)
                {
                    try
                    {
                        sqlConnections.Add(WebConfigurationManager.ConnectionStrings[i].Name, new SqlConnection(WebConfigurationManager.ConnectionStrings[i].ConnectionString));
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return sqlConnections;
        }
    }
    #endregion

    #region Public methods
    public bool ExecuteNonQuery(string SQLString)
    {
        int affectedRows;
        return ExecuteNonQuery(SQLString, out affectedRows);
    }

    public bool ExecuteNonQuery(string SQLString, out int AffectedRows)
    {
        return ExecuteNonQuery(SQLString, SqlConnections[DefaultConnectionName], out AffectedRows);
    }

    public bool ExecuteNonQuery(string SQLString, SqlConnection SQLConnection)
    {
        int affectedRows;
        return ExecuteNonQuery(SQLString, SQLConnection, out affectedRows);
    }

    public bool ExecuteNonQuery(string SQLString, IEnumerable<SqlParameter> SQLParameters)
    {
        int affectedRows;
        return ExecuteNonQuery(SQLString, SQLParameters, out affectedRows);
    }

    public bool ExecuteNonQuery(string SQLString, SqlConnection SQLConnection, out int AffectedRows)
    {
        return ExecuteNonQuery(SQLString, null, SQLConnection, out AffectedRows);
    }

    public bool ExecuteNonQuery(string SQLString, IEnumerable<SqlParameter> SQLParameters, out int AffectedRows)
    {
        return ExecuteNonQuery(SQLString, SQLParameters, SqlConnections[DefaultConnectionName], out AffectedRows);
    }

    public bool ExecuteNonQuery(string SQLString, IEnumerable<SqlParameter> SQLParameters, SqlConnection SQLConnection)
    {
        int affectedRows;
        return ExecuteNonQuery(SQLString, SQLParameters, SQLConnection, out affectedRows);
    }

    public bool ExecuteNonQuery(string SQLString, IEnumerable<SqlParameter> SQLParameters, SqlConnection SQLConnection, out int AffectedRows)
    {
        bool success = false;
        AffectedRows = 0;

        try
        {
            // Open connection
            SQLConnection.Open();

            // Execute SQL
            SqlCommand cmd = new SqlCommand(SQLString, SQLConnection);
            if (SQLParameters != null && SQLParameters.Any())
                cmd.Parameters.AddRange(SQLParameters.ToArray());
            AffectedRows = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            // Set success
            success = true;
        }
        catch (Exception ex)
        {
        }
        finally
        {
            // Close connection
            if (SQLConnection != null && SQLConnection.State != ConnectionState.Closed)
                SQLConnection.Close();
        }

        return success;
    }

    public bool ExecuteQuery(string SQLString, out DataTable Data)
    {
        return ExecuteQuery(SQLString, SqlConnections[DefaultConnectionName], out Data);
    }

    public bool ExecuteQuery(string SQLString, SqlConnection SQLConnection, out DataTable DataTable)
    {
        return ExecuteQuery(SQLString, null, SQLConnection, out DataTable);
    }

    public bool ExecuteQuery(string SQLString, IEnumerable<SqlParameter> SQLParameters, out DataTable Data)
    {
        return ExecuteQuery(SQLString, SQLParameters, SqlConnections[DefaultConnectionName], out Data);
    }

    public bool ExecuteQuery(string SQLString, IEnumerable<SqlParameter> SQLParameters, SqlConnection SQLConnection, out DataTable DataTable)
    {
        bool success = false;
        DataTable = null;

        try
        {
            // Open connection
            SQLConnection.Open();

            // Execute SQL
            SqlDataAdapter ad = new SqlDataAdapter(SQLString, SQLConnection);
            if (SQLParameters != null && SQLParameters.Any())
                ad.SelectCommand.Parameters.AddRange(SQLParameters.ToArray());
            DataTable = new DataTable();
            ad.Fill(DataTable);
            ad.SelectCommand.Parameters.Clear();

            // Set success
            success = true;
        }
        catch (Exception ex)
        {
            DataTable = null;
        }
        finally
        {
            // Close connection
            if (SQLConnection != null && SQLConnection.State != ConnectionState.Closed)
                SQLConnection.Close();
        }

        return success;
    }

    public bool ExecuteScalar(string SQLString, out object Object)
    {
        return ExecuteScalar(SQLString, SqlConnections[DefaultConnectionName], out Object);
    }

    public bool ExecuteScalar(string SQLString, SqlConnection SQLConnection, out object Object)
    {
        return ExecuteScalar(SQLString, null, SQLConnection, out Object);
    }

    public bool ExecuteScalar(string SQLString, IEnumerable<SqlParameter> SQLParameters, out object Object)
    {
        return ExecuteScalar(SQLString, SQLParameters, SqlConnections[DefaultConnectionName], out Object);
    }

    public bool ExecuteScalar(string SQLString, IEnumerable<SqlParameter> SQLParameters, SqlConnection SQLConnection, out object Object)
    {
        bool success = false;
        Object = null;

        try
        {
            // Open connection
            SQLConnection.Open();

            // Execute SQL
            SqlCommand cmd = new SqlCommand(SQLString, SQLConnection);
            if (SQLParameters != null && SQLParameters.Any())
                cmd.Parameters.AddRange(SQLParameters.ToArray());
            Object = cmd.ExecuteScalar();
            cmd.Parameters.Clear();

            // Set success
            success = true;
        }
        catch (Exception ex)
        {
            Object = null;
        }
        finally
        {
            // Close connection
            if (SQLConnection != null && SQLConnection.State != ConnectionState.Closed)
                SQLConnection.Close();
        }

        return success;
    }
    #endregion
}

public static class TXLExtensions
{
    #region Variables
    private static string encryptionHashAlgorithm = string.Empty;
    private static int encryptionKeySize = 0;
    private static int encryptionPasswordIteration = 0;
    private static string encryptionSalt = string.Empty;
    private static string encryptionVector = string.Empty;
    #endregion

    #region Properties
    public static string EncryptionHashAlgorithm
    {
        get
        {
            if (string.IsNullOrWhiteSpace(encryptionHashAlgorithm))
            {
                string key = "EncryptionHashAlgorithm";
                if (!string.IsNullOrWhiteSpace(WebConfigurationManager.AppSettings[key]))
                    encryptionHashAlgorithm = WebConfigurationManager.AppSettings[key];
                else if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[key]))
                    encryptionHashAlgorithm = ConfigurationManager.AppSettings[key];
            }

            if (string.IsNullOrWhiteSpace(encryptionHashAlgorithm))
                encryptionHashAlgorithm = "SHA512";

            return encryptionHashAlgorithm;
        }
    }

    public static int EncryptionKeySize
    {
        get
        {
            if (encryptionKeySize == 0)
            {
                string key = "EncryptionKeySize";
                if (!string.IsNullOrWhiteSpace(WebConfigurationManager.AppSettings[key]))
                    encryptionKeySize = Convert.ToInt32(WebConfigurationManager.AppSettings[key]);
                else if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[key]))
                    encryptionKeySize = Convert.ToInt32(ConfigurationManager.AppSettings[key]);
            }

            if (encryptionKeySize == 0)
                encryptionKeySize = 256;

            return encryptionKeySize;
        }
    }

    public static int EncryptionPasswordIteration
    {
        get
        {
            if (encryptionPasswordIteration == 0)
            {
                string key = "EncryptionPasswordIteration";
                if (!string.IsNullOrWhiteSpace(WebConfigurationManager.AppSettings[key]))
                    encryptionPasswordIteration = Convert.ToInt32(WebConfigurationManager.AppSettings[key]);
                else if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[key]))
                    encryptionPasswordIteration = Convert.ToInt32(ConfigurationManager.AppSettings[key]);
            }

            if (encryptionPasswordIteration == 0)
                encryptionPasswordIteration = 256;

            return encryptionPasswordIteration;
        }
    }

    public static string EncryptionSalt
    {
        get
        {
            if (string.IsNullOrWhiteSpace(encryptionSalt))
            {
                string key = "EncryptionSalt";
                if (!string.IsNullOrWhiteSpace(WebConfigurationManager.AppSettings[key]))
                    encryptionSalt = WebConfigurationManager.AppSettings[key];
                else if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[key]))
                    encryptionSalt = ConfigurationManager.AppSettings[key];
            }

            if (string.IsNullOrWhiteSpace(encryptionSalt))
                encryptionSalt = "nUc@phuS";

            return encryptionSalt;
        }
    }

    public static string EncryptionVector
    {
        get
        {
            if (string.IsNullOrWhiteSpace(encryptionVector))
            {
                string key = "EncryptionVector";
                if (!string.IsNullOrWhiteSpace(WebConfigurationManager.AppSettings[key]))
                    encryptionVector = WebConfigurationManager.AppSettings[key];
                else if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[key]))
                    encryptionVector = ConfigurationManager.AppSettings[key];
            }

            if (string.IsNullOrWhiteSpace(encryptionVector))
                encryptionVector = "baqa3H?BraGakePu";

            return encryptionVector;
        }
    }
    #endregion

    #region DataTable extension methods
    public static List<T> ToList<T>(this DataTable DataTable)
    {
        List<string> columnNames = DataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
        PropertyInfo[] props = typeof(T).GetProperties();

        List<T> list = DataTable.AsEnumerable().Select(row =>
        {
            T obj = Activator.CreateInstance<T>();

            foreach (PropertyInfo prop in props)
            {
                if (columnNames.Contains(prop.Name.ToLower()))
                {
                    if (prop.PropertyType.BaseType == typeof(Enum))
                        prop.SetValue(obj, Enum.Parse(prop.PropertyType, row[prop.Name.ToLower()].ToString(), true));
                    else
                        prop.SetValue(obj, row[prop.Name.ToLower()] == DBNull.Value ? null : Convert.ChangeType(row[prop.Name.ToLower()], obj.GetType().GetProperty(prop.Name).PropertyType));
                }
            }

            return obj;
        }).ToList();

        return list;
    }
    #endregion

    #region Object extension methods
    public static bool IsDate(this object Object)
    {
        bool success = false;

        try
        {
            DateTime date;
            List<Type> types = new List<Type> { typeof(DateTime) };
            success = (types.Contains(Object.GetType()) || (Object != null && DateTime.TryParse(Object.ToString(), out date)));
        }
        catch (Exception ex)
        {
        }

        return success;
    }

    public static bool IsNumber(this object Object, bool IncludeDividers = false)
    {
        bool success = false;

        try
        {
            List<Type> types = new List<Type> { typeof(Decimal), typeof(Double), typeof(Int16), typeof(Int32), typeof(Int64) };
            success = (types.Contains(Object.GetType()) || (Object != null && Regex.IsMatch(Object.ToString(), (IncludeDividers ? @"^-?\d+((.|,)\d+)?$" : @"^-?\d+$"))));
        }
        catch (Exception ex)
        {
        }

        return success;
    }

    public static T Merge<T>(this T Target, object Source, bool AllowNull = false)
    {
        return Target.Merge(Source, null, AllowNull);
    }

    public static T Merge<T>(this T Target, object Source, IEnumerable<string> ExcludeProperties, bool AllowNull = false)
    {
        if (ExcludeProperties == null)
            ExcludeProperties = new string[0];

        Dictionary<PropertyInfo, PropertyInfo> pis = new Dictionary<PropertyInfo, PropertyInfo>();
        PropertyInfo[] target = Target.GetType().GetProperties();
        PropertyInfo[] source = Source.GetType().GetProperties();

        foreach (PropertyInfo pi in target)
        {
            if (!ExcludeProperties.Any(x => x.ToLower() == pi.Name.ToLower()))
            {
                var temp = source.FirstOrDefault(x => x.Name.ToLower() == pi.Name.ToLower() && x.PropertyType == x.PropertyType);
                if (temp != null)
                    pis.Add(pi, temp);
            }
        }

        foreach (KeyValuePair<PropertyInfo, PropertyInfo> kvp in pis)
        {
            var value = kvp.Value.GetValue(Source);
            if (AllowNull || value != null)
                kvp.Key.SetValue(Target, value);
        }

        return Target;
    }

    public static void ToEmptyStrings(this object Object)
    {
        foreach (PropertyInfo pi in Object.GetType().GetProperties())
            if (pi.PropertyType == typeof(String) && pi.GetValue(Object) == null)
                pi.SetValue(Object, "");
    }

    public static List<SqlParameter> ToSqlParameters(this object Object)
    {
        return Object.ToSqlParameters(null, null);
    }

    public static List<SqlParameter> ToSqlParameters(this object Object, bool LowerCaseNames)
    {
        return Object.ToSqlParameters(null, null, LowerCaseNames);
    }

    public static List<SqlParameter> ToSqlParameters(this object Object, string AppendTag)
    {
        return Object.ToSqlParameters(AppendTag, null);
    }

    public static List<SqlParameter> ToSqlParameters(this object Object, IEnumerable<string> ExcludeProperties)
    {
        return Object.ToSqlParameters(null, ExcludeProperties);
    }

    public static List<SqlParameter> ToSqlParameters(this object Object, string AppendTag, bool LowerCaseNames)
    {
        return Object.ToSqlParameters(AppendTag, null, LowerCaseNames);
    }

    public static List<SqlParameter> ToSqlParameters(this object Object, IEnumerable<string> ExcludeProperties, bool LowerCaseNames)
    {
        return Object.ToSqlParameters(null, ExcludeProperties, LowerCaseNames);
    }

    public static List<SqlParameter> ToSqlParameters(this object Object, string AppendTag, IEnumerable<string> ExcludeProperties, bool LowerCaseNames = true)
    {
        List<SqlParameter> sqlParams = new List<SqlParameter>();

        Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>();
        typeMap[typeof(byte)] = DbType.Byte;
        typeMap[typeof(sbyte)] = DbType.SByte;
        typeMap[typeof(short)] = DbType.Int16;
        typeMap[typeof(ushort)] = DbType.Int16;
        typeMap[typeof(int)] = DbType.Int32;
        typeMap[typeof(uint)] = DbType.Int32;
        typeMap[typeof(long)] = DbType.Int64;
        typeMap[typeof(ulong)] = DbType.Int64;
        typeMap[typeof(float)] = DbType.Single;
        typeMap[typeof(double)] = DbType.Double;
        typeMap[typeof(decimal)] = DbType.Decimal;
        typeMap[typeof(bool)] = DbType.Boolean;
        typeMap[typeof(string)] = DbType.String;
        typeMap[typeof(char)] = DbType.StringFixedLength;
        typeMap[typeof(Guid)] = DbType.Guid;
        typeMap[typeof(DateTime)] = DbType.DateTime;
        typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
        typeMap[typeof(byte[])] = DbType.Binary;
        typeMap[typeof(byte?)] = DbType.Byte;
        typeMap[typeof(sbyte?)] = DbType.SByte;
        typeMap[typeof(short?)] = DbType.Int16;
        typeMap[typeof(ushort?)] = DbType.UInt16;
        typeMap[typeof(int?)] = DbType.Int32;
        typeMap[typeof(uint?)] = DbType.Int32;
        typeMap[typeof(long?)] = DbType.Int64;
        typeMap[typeof(ulong?)] = DbType.Int64;
        typeMap[typeof(float?)] = DbType.Single;
        typeMap[typeof(double?)] = DbType.Double;
        typeMap[typeof(decimal?)] = DbType.Decimal;
        typeMap[typeof(bool?)] = DbType.Boolean;
        typeMap[typeof(char?)] = DbType.StringFixedLength;
        typeMap[typeof(Guid?)] = DbType.Guid;
        typeMap[typeof(DateTime?)] = DbType.DateTime;
        typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;

        if (ExcludeProperties == null)
            ExcludeProperties = new string[0];

        foreach (PropertyInfo pi in Object.GetType().GetProperties())
        {
            if ((typeMap.ContainsKey(pi.PropertyType) || pi.PropertyType.BaseType == typeof(Enum)) && !ExcludeProperties.Any(x => x.ToLower() == pi.Name.ToLower()))
            {
                if (pi.PropertyType == typeof(DateTime) && ((DateTime)pi.GetValue(Object) < SqlDateTime.MinValue.Value || (DateTime)pi.GetValue(Object) > SqlDateTime.MaxValue.Value))
                    sqlParams.Add(new SqlParameter { DbType = typeMap[typeof(DateTime)], ParameterName = "@" + pi.Name.ToLower() + AppendTag, Value = DBNull.Value });
                else if (pi.PropertyType == typeof(DateTime?) && ((DateTime?)pi.GetValue(Object)).HasValue && (((DateTime?)pi.GetValue(Object)).Value < SqlDateTime.MinValue.Value || ((DateTime?)pi.GetValue(Object)).Value > SqlDateTime.MaxValue.Value))
                    sqlParams.Add(new SqlParameter { DbType = typeMap[typeof(DateTime)], ParameterName = "@" + pi.Name.ToLower() + AppendTag, Value = DBNull.Value });
                else if (pi.PropertyType.BaseType == typeof(Enum))
                    sqlParams.Add(new SqlParameter { DbType = typeMap[typeof(Int32)], ParameterName = "@" + pi.Name.ToLower() + AppendTag, Value = (int)pi.GetValue(Object) });
                else
                    sqlParams.Add(new SqlParameter { DbType = typeMap[pi.PropertyType], ParameterName = "@" + pi.Name.ToLower() + AppendTag, Value = (pi.GetValue(Object) ?? DBNull.Value) });
            }
        }

        return sqlParams;
    }
    #endregion

    #region String extension methods
    public static string Decrypt(this string String, string Password)
    {
        string s = string.Empty;

        try
        {
            s = decrypt(String, Password, EncryptionSalt, EncryptionHashAlgorithm, EncryptionPasswordIteration, EncryptionVector, EncryptionKeySize);
        }
        catch (Exception ex)
        {
        }

        return s;
    }

    public static string Encrypt(this string String, string Password)
    {
        string s = string.Empty;

        try
        {
            s = encrypt(String, Password, EncryptionSalt, EncryptionHashAlgorithm, EncryptionPasswordIteration, EncryptionVector, EncryptionKeySize);
        }
        catch (Exception ex)
        {
        }

        return s;
    }

    public static string ToSHA256HashString(this string String)
    {
        return String.ToSHA256HashString("X2");
    }

    public static string ToSHA256HashString(this string String, string Format)
    {
        HashAlgorithm algorithm = SHA256.Create();
        StringBuilder sb = new StringBuilder();
        foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(String)))
            sb.Append(b.ToString(Format));
        return sb.ToString();
    }

    public static string ToSHA512HashString(this string String)
    {
        return String.ToSHA512HashString("X2");
    }

    public static string ToSHA512HashString(this string String, string Format)
    {
        HashAlgorithm algorithm = SHA512.Create();
        StringBuilder sb = new StringBuilder();
        foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(String)))
            sb.Append(b.ToString(Format));
        return sb.ToString();
    }

    public static string ToSQLSafe(this string String)
    {
        return String.Replace("'", "''");
    }
    #endregion

    #region Private methods
    private static string decrypt(string cipherText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
    {
        // Convert strings defining encryption key characteristics into byte
        // arrays. Let us assume that strings only contain ASCII codes.
        // If strings include Unicode characters, use Unicode, UTF7, or UTF8
        // encoding.
        byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
        byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

        // Convert our ciphertext into a byte array.
        byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

        // First, we must create a password, from which the key will be 
        // derived. This password will be generated from the specified 
        // passphrase and salt value. The password will be created using
        // the specified hash algorithm. Password creation can be done in
        // several iterations.
        PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

        // Use the password to generate pseudo-random bytes for the encryption
        // key. Specify the size of the key in bytes (instead of bits).
        byte[] keyBytes = password.GetBytes(keySize / 8);

        // Create uninitialized Rijndael encryption object.
        RijndaelManaged symmetricKey = new RijndaelManaged();

        // It is reasonable to set encryption mode to Cipher Block Chaining
        // (CBC). Use default options for other symmetric key parameters.
        symmetricKey.Mode = CipherMode.CBC;

        // Generate decryptor from the existing key bytes and initialization 
        // vector. Key size will be defined based on the number of the key 
        // bytes.
        ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

        // Define memory stream which will be used to hold encrypted data.
        MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

        // Define cryptographic stream (always use Read mode for encryption).
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        // Since at this point we don't know what the size of decrypted data
        // will be, allocate the buffer long enough to hold ciphertext;
        // plaintext is never longer than ciphertext.
        byte[] plainTextBytes = new byte[cipherTextBytes.Length];

        // Start decrypting.
        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

        // Close both streams.
        memoryStream.Close();
        cryptoStream.Close();

        // Convert decrypted data into a string. 
        // Let us assume that the original plaintext string was UTF8-encoded.
        string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

        // Return decrypted string.   
        return plainText;
    }

    private static string encrypt(string plainText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
    {
        // Convert strings into byte arrays.
        // Let us assume that strings only contain ASCII codes.
        // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
        // encoding.
        byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
        byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

        // Convert our plaintext into a byte array.
        // Let us assume that plaintext contains UTF8-encoded characters.
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        // First, we must create a password, from which the key will be derived.
        // This password will be generated from the specified passphrase and 
        // salt value. The password will be created using the specified hash 
        // algorithm. Password creation can be done in several iterations.
        PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

        // Use the password to generate pseudo-random bytes for the encryption
        // key. Specify the size of the key in bytes (instead of bits).
        byte[] keyBytes = password.GetBytes(keySize / 8);

        // Create uninitialized Rijndael encryption object.
        RijndaelManaged symmetricKey = new RijndaelManaged();

        // It is reasonable to set encryption mode to Cipher Block Chaining
        // (CBC). Use default options for other symmetric key parameters.
        symmetricKey.Mode = CipherMode.CBC;

        // Generate encryptor from the existing key bytes and initialization 
        // vector. Key size will be defined based on the number of the key 
        // bytes.
        ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

        // Define memory stream which will be used to hold encrypted data.
        MemoryStream memoryStream = new MemoryStream();

        // Define cryptographic stream (always use Write mode for encryption).
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        // Start encrypting.
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

        // Finish encrypting.
        cryptoStream.FlushFinalBlock();

        // Convert our encrypted data from a memory stream into a byte array.
        byte[] cipherTextBytes = memoryStream.ToArray();

        // Close both streams.
        memoryStream.Close();
        cryptoStream.Close();

        // Convert encrypted data into a base64-encoded string.
        string cipherText = Convert.ToBase64String(cipherTextBytes);

        // Return encrypted string.
        return cipherText;
    }
    #endregion
}

public class TXLFilteringPagingSorting
{
    #region Variables
    private int defaultPageLimit = -1;
    private string defaultSortColumn = string.Empty;
    private int itemCount = -1;
    private int pageLimit = -1;
    private string pageLimitParamName = string.Empty;
    private string pageParamName = string.Empty;
    private string searchParamName = string.Empty;
    private string sortColumnParamName = string.Empty;
    private Dictionary<string, string> sortColumns = new Dictionary<string, string>();
    private string sortDirectionParamName = string.Empty;
    #endregion

    #region Properties
    public int DefaultPageLimit
    {
        get
        {
            return defaultPageLimit;
        }
        set
        {
            defaultPageLimit = value;
        }
    }

    public string DefaultSortColumn
    {
        get
        {
            return defaultSortColumn;
        }
        set
        {
            defaultSortColumn = value;
        }
    }

    public int ItemCount
    {
        get
        {
            return itemCount;
        }
        set
        {
            itemCount = value;
        }
    }

    public int PageCount
    {
        get
        {
            return (int)Math.Ceiling((double)ItemCount / PageLimit);
        }
    }

    public int PageCurrent
    {
        get
        {
            int page;
            return (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.Params[PageParamName]) && int.TryParse(HttpContext.Current.Request.Params[PageParamName], out page) && page <= PageCount ? page : 1);
        }
    }

    public int PageIndex
    {
        get
        {
            return (PageCurrent - 1);
        }
    }

    public int PageLimit
    {
        get
        {
            int limit;
            if (pageLimit == -1)
                pageLimit = (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.Params[PageLimitParamName]) && int.TryParse(HttpContext.Current.Request.Params[PageLimitParamName], out limit) && limit > 0 ? limit : DefaultPageLimit);
            return pageLimit;
        }
        set
        {
            pageLimit = value;
        }
    }

    public string PageLimitParamName
    {
        get
        {
            return pageLimitParamName;
        }
        set
        {
            pageLimitParamName = value;
        }
    }

    public int PageNext
    {
        get
        {
            return Math.Min(PageCount, (PageCurrent + 1));
        }
    }

    public string PageParamName
    {
        get
        {
            return pageParamName;
        }
        set
        {
            pageParamName = value;
        }
    }

    public int PagePrevious
    {
        get
        {
            return Math.Max(1, (PageCurrent - 1));
        }
    }

    public string SearchParamName
    {
        get
        {
            return searchParamName;
        }
        set
        {
            searchParamName = value;
        }
    }

    public string SortColumn
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.Params[SortColumnParamName]) && SortColumns.ContainsKey(HttpContext.Current.Request.Params[SortColumnParamName].ToLower()))
                return HttpContext.Current.Request.Params[SortColumnParamName].ToLower();
            else
                return DefaultSortColumn;
        }
    }

    public string SortColumnParamName
    {
        get
        {
            return sortColumnParamName;
        }
        set
        {
            sortColumnParamName = value;
        }
    }

    public Dictionary<string, string> SortColumns
    {
        get
        {
            return sortColumns;
        }
        set
        {
            sortColumns = value;
        }
    }

    public string SortDirection
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.Params[SortDirectionParamName]) && (HttpContext.Current.Request.Params[SortDirectionParamName].ToLower() == "asc" || HttpContext.Current.Request.Params[SortDirectionParamName].ToLower() == "desc"))
                return HttpContext.Current.Request.Params[SortDirectionParamName].ToLower();
            else if (SortColumn != "")
                return SortColumns[SortColumn].ToLower();
            else
                return SortColumns[DefaultSortColumn].ToLower();
        }
    }

    public string SortDirectionParamName
    {
        get
        {
            return sortDirectionParamName;
        }
        set
        {
            sortDirectionParamName = value;
        }
    }
    #endregion

    #region Public methods
    public DataTable CreatePaging(string SelectedCssClass)
    {
        // Create paging
        DataTable paging = new DataTable();
        paging.Columns.Add("class", typeof(String));
        paging.Columns.Add(PageParamName, typeof(Int32));
        paging.Columns.Add("text", typeof(String));

        if (PageCount > 0)
        {
            // Calculate start and end
            int x = 2;
            int before = Math.Min(x, PageIndex);
            int after = Math.Min(x, (PageCount - PageIndex - 1));

            if (before < x)
            {
                after = Math.Min((x + (x - before)), (PageCount - PageIndex - 1));
            }
            else if (after < x)
            {
                before = Math.Min((x + (x - after)), PageIndex);
            }

            int start = (PageIndex - before);
            int end = (PageIndex + after);

            for (int i = start; i <= end; i++)
            {
                // Add pages
                DataRow row = paging.NewRow();
                row["class"] = (i == PageIndex ? SelectedCssClass : "");
                row[PageParamName] = (i + 1);
                row["text"] = (i + 1);
                paging.Rows.Add(row);
            }
        }
        else
        {
            // Add pages
            DataRow row = paging.NewRow();
            row["class"] = SelectedCssClass;
            row[PageParamName] = 1;
            row["text"] = 1;
            paging.Rows.Add(row);
        }

        return paging;
    }

    public string GetNumericParams(string ParamName, int Value)
    {
        string qs = GetQueryStringValues(ParamName, PageParamName);

        string value = string.Empty;
        if (HttpContext.Current.Request.QueryString.AllKeys.Contains(ParamName))
            value = HttpContext.Current.Request.QueryString[ParamName];
        List<int> values = value.Split(',').Where(x => Regex.IsMatch(x, @"^-?\d+$")).Select(x => Convert.ToInt32(x)).ToList();
        if (values.Contains(Value))
            values = values.Where(x => x != Value).ToList();
        else
            values.Add(Value);
        values.Sort();
        value = string.Join(",", values);

        return string.Format("{0}{1}{2}={3}", qs, (qs != "" ? "&" : ""), ParamName, HttpUtility.UrlEncode(value));
    }

    public string GetNumericParamSelected(string ParamName, int Value, string CssClass)
    {
        string value = string.Empty;
        if (HttpContext.Current.Request.QueryString.AllKeys.Contains(ParamName))
            value = HttpContext.Current.Request.QueryString[ParamName];
        List<int> values = value.Split(',').Where(x => Regex.IsMatch(x, @"^-?\d+$")).Select(x => Convert.ToInt32(x)).ToList();
        return (values.Contains(Value) ? CssClass : string.Empty);
    }

    public string GetPageLimitParam(int Limit)
    {
        string qs = GetQueryStringValues(PageLimitParamName, PageParamName);
        return string.Format("{0}{1}{2}={3}", qs, (qs != "" ? "&" : ""), PageLimitParamName, Limit);
    }

    public string GetPageParam(int Page)
    {
        string qs = GetQueryStringValues(PageParamName);
        return string.Format("{0}{1}{2}={3}", qs, (qs != "" ? "&" : ""), PageParamName, Page);
    }

    public string GetQueryStringValues(params string[] Excludes)
    {
        string qs = string.Empty;
        foreach (string key in HttpContext.Current.Request.QueryString.AllKeys.Where(x => !Excludes.Contains(x)))
            qs += string.Format("{0}{1}={2}", (qs != "" ? "&" : ""), key, HttpUtility.UrlEncode(HttpContext.Current.Request.QueryString[key]));
        return qs;
    }

    public string GetSearchParam(string Search)
    {
        string qs = GetQueryStringValues(SearchParamName, PageParamName);
        return string.Format("{0}{1}{2}={3}", qs, (qs != "" ? "&" : ""), SearchParamName, HttpUtility.UrlEncode(Search));
    }

    public string GetSortParams(string Column)
    {
        string qs = GetQueryStringValues(SortColumnParamName, SortDirectionParamName, PageParamName);
        string dir = (Column == SortColumn ? (SortDirection == "asc" ? "desc" : "asc") : (SortColumns.ContainsKey(Column) ? SortColumns[Column] : "asc"));
        return string.Format("{0}{1}{2}={3}&{4}={5}", qs, (qs != "" ? "&" : ""), SortColumnParamName, HttpUtility.UrlEncode(Column), SortDirectionParamName, HttpUtility.UrlEncode(dir));
    }

    public string GetStringParam(string ParamName, string Value)
    {
        string qs = GetQueryStringValues(ParamName, PageParamName);
        return string.Format("{0}{1}{2}={3}", qs, (qs != "" ? "&" : ""), ParamName, HttpUtility.UrlEncode(Value));
    }

    public string GetStringParamSelected(string ParamName, string Value, string CssClass)
    {
        string value = string.Empty;
        if (HttpContext.Current.Request.QueryString.AllKeys.Contains(ParamName))
            value = HttpContext.Current.Request.QueryString[ParamName];
        return (value == Value ? CssClass : string.Empty);
    }

    public string GetStringParamsSelected(string ParamName, string Value, string CssClass)
    {
        string value = string.Empty;
        if (HttpContext.Current.Request.QueryString.AllKeys.Contains(ParamName))
            value = HttpContext.Current.Request.QueryString[ParamName];
        List<string> values = value.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();
        return (values.Contains(Value) ? CssClass : string.Empty);
    }
    #endregion
}