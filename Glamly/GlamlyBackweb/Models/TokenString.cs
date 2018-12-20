using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlamlyBackweb.Models
{
    public class TokenString
    {
        #region Enums
        public enum TokenType
        {
            AccessToken,
            RefreshToken,
            Unknown
        }
        #endregion

        #region Variables
        private DateTime createDate;
        private TokenType type = TokenType.Unknown;
        private long userId = 0;
        private string[] values = new string[0];
        #endregion

        #region Properties
        /// <summary>
        /// Create date of token string
        /// </summary>
        public DateTime CreateDate
        {
            get { return createDate; }
        }

        /// <summary>
        /// Token type of token string
        /// </summary>
        public TokenType Type
        {
            get { return type; }
        }

        /// <summary>
        /// User id of token string
        /// </summary>
        public long UserId
        {
            get { return userId; }
        }

        /// <summary>
        /// Values of token string
        /// </summary>
        public string[] Values
        {
            get { return values ?? new string[0]; }
        }
        #endregion

        #region Constructor
        public TokenString(string Token)
        {
            try
            {
                // Decrypt token
                string[] data = Token.Decrypt("TokenString").Split('|');

                // Set values
                createDate = DateTime.SpecifyKind(DateTime.ParseExact(data[0], "yyyy-MM-ddTHH:mm:ss.fff", null), DateTimeKind.Utc);
                type = (TokenType)Enum.Parse(typeof(TokenType), data[1]);
                userId = Convert.ToInt64(data[2]);
                values = data.Skip(3).ToArray();
            }
            catch (Exception ex)
            {
            }
        }

        public TokenString(TokenType Type, long UserId, params string[] Values)
        {
            // Set values
            createDate = DateTime.UtcNow;
            type = Type;
            userId = UserId;
            values = Values;
        }
        #endregion

        #region Public methods
        public static TokenString AccessToken(long UserId, int ExpiresIn, string RefreshToken)
        {
            return new TokenString(TokenString.TokenType.AccessToken, UserId, ExpiresIn.ToString(), RefreshToken);
        }

        public static TokenString RefreshToken(long UserId)
        {
            return new TokenString(TokenString.TokenType.RefreshToken, UserId);
        }

        public new string ToString()
        {
            string s = null;

            // Build token
            s = string.Format("{0}|{1}|{2}", CreateDate.ToString("yyyy-MM-ddTHH:mm:ss.fff"), Type, UserId);
            if (Values.Length > 0)
                s += "|" + string.Join("|", Values);

            // Encrypt token
            s = s.Encrypt("TokenString");

            return s;
        }
        #endregion
    }
}