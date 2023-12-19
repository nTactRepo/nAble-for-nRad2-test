using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace CommonLibrary.Model.User
{
    public class UserManager<T> where T : BaseUser, new()
    {
        #region Events

        public event Action Changed;

        #endregion

        #region Constants

        private const string NTactUsername = "nTact";
        private const string NTactPassword = "75238";

        #endregion

        #region Properties

        [XmlArray]
        public List<T> Users { get; private set; } = new List<T>();

        [XmlIgnore]
        public string Filename { get; set; } = "";

        #endregion

        #region Functions

        #region Constructors

        public UserManager() { }

        #endregion

        #region Serialization

        public static UserManager<T> Load(string filename)
        {
            UserManager<T> userAccess = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UserManager<T>));

                using (var reader = new StreamReader(filename))
                {
                    userAccess = (UserManager<T>)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine("ERROR: Could not read User File - }" + ex.ToString(), "ERROR");
                userAccess = null;
            }

            if (userAccess != null)
            {
                userAccess.Filename = filename;
            }

            return userAccess;
        }

        public bool Save(string filename = "")
        {
            StreamWriter writer = null;
            bool saved = false;

            if (string.IsNullOrEmpty(filename))
            {
                filename = Filename;

                // If still empty, then we have nothing.  Throw
                if (string.IsNullOrEmpty(filename))
                {
                    throw new ArgumentNullException(Filename, "Cannot save a user manager until a filename has been set!");
                }
            }

            try
            {
                var dir = Path.GetDirectoryName(filename);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                XmlSerializer serializer = new XmlSerializer(typeof(UserManager<T>));
                writer = new StreamWriter(filename, append: false);
                serializer.Serialize(writer, this);
                writer.Close();
                saved = true;
                Changed?.Invoke();
                Filename = filename;
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"UserManager: Could not save user file - {ex}", "ERROR");

                if (writer != null)
                {
                    writer.Close();
                }
            }

            return saved;
        }

        #endregion

        #region Public Functions

        public T ValidateCredentials(string username, string password)
        {
            if (username.ToLower() == NTactUsername.ToLower() && password == NTactPassword)
            {
                return GetNTactUser();
            }

            return Users.Find(user => user.Validate(username, password));
        }

        public T GetNotLoggedOnUser()
        {
            return new T() { Name = BaseUser.NotLoggedOnName };
        }

        public T GetNTactUser()
        {
            T user = new T();
            user.Name = NTactUsername;
            user.Password = NTactPassword;
            user.FillInNTactUser();
            return user;
        }

        public bool ContainsUserName(string username)
        {
            return Users.Exists(user => user.Name.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        }

        public void RemoveUser(T user)
        {
            Users.Remove(user);
        }

        #endregion

        #endregion
    }
}
