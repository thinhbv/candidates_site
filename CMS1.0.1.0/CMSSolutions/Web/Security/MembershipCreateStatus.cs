namespace CMSSolutions.Web.Security
{
    public enum MembershipCreateStatus
    {
        /// <summary>
        /// The user was successfully created.
        /// </summary>
        Success,

        /// <summary>
        /// The user name was not found in the database.
        /// </summary>
        InvalidUserName,

        /// <summary>
        /// The password is not formatted correctly.
        /// </summary>
        InvalidPassword,

        /// <summary>
        /// The password question is not formatted correctly.
        /// </summary>
        InvalidQuestion,

        /// <summary>
        /// The password answer is not formatted correctly.
        /// </summary>
        InvalidAnswer,

        /// <summary>
        /// The e-mail address is not formatted correctly.
        /// </summary>
        InvalidEmail,

        /// <summary>
        /// The user name already exists in the database for the application.
        /// </summary>
        DuplicateUserName,

        /// <summary>
        /// The e-mail address already exists in the database for the application.
        /// </summary>
        DuplicateEmail,

        /// <summary>
        /// The user was not created, for a reason defined by the provider.
        /// </summary>
        UserRejected,

        /// <summary>
        /// The provider user key is of an invalid type or format.
        /// </summary>
        InvalidProviderUserKey,

        /// <summary>
        /// The provider user key already exists in the database for the application.
        /// </summary>
        DuplicateProviderUserKey,

        /// <summary>
        /// The provider returned an error that is not described by other MembershipCreateStatus enumeration values.
        /// </summary>
        ProviderError,
    }
}