using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Data;

namespace CommonLibrary.Model.User
{
    public class CellUser : BaseUser, IComparable<CellUser>, IEquatable<CellUser>
    {
        #region Constants

        public static readonly CellUser NoUser = new CellUser(NotLoggedOnName);

        #endregion

        #region Properties

        public bool UserManagement { get; set; } = false;
        public bool ModifySystemParameters { get; set; } = false;
        public bool ManageRecipes { get; set; } = false;
        public bool ChangeModes { get; set; } = false;
        public bool WorkInMaintMode { get; set; } = false;
        public bool WorkInManualMode { get; set; } = false;
        public bool UnlockDoors { get; set; } = false;

        #endregion

        #region Functions

        public CellUser() { }

        public CellUser(string name) : base(name) { }

        public override void FillInNTactUser()
        {
            UserManagement = true;
            ModifySystemParameters = true;
            ManageRecipes = true;
            ChangeModes = true;
            WorkInMaintMode = true;
            WorkInManualMode = true;
            UnlockDoors = true;
        }

        #region IComparable

        public virtual int CompareTo(CellUser other)
        {
            return base.CompareTo(other);
        }

        #endregion

        #region IEquitable

        public virtual bool Equals(CellUser other)
        {
            return base.Equals(other) &&
                   UserManagement.Equals(other?.UserManagement) &&
                   ModifySystemParameters.Equals(other?.ModifySystemParameters) &&
                   ManageRecipes.Equals(other?.ManageRecipes) &&
                   ChangeModes.Equals(other?.ChangeModes) &&
                   WorkInMaintMode.Equals(other?.WorkInMaintMode) &&
                   WorkInManualMode.Equals(other?.WorkInManualMode) &&
                   UnlockDoors.Equals(other?.UnlockDoors);
        }

        #endregion

        #endregion
    }
}
