using PerspectiveV.Common.Extensions;

namespace PerspectiveV.Common; 
/// <summary>
/// Represents the AppException type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AppException.
/// var type = typeof(AppException);
/// ]]></code>
/// </example>
public class AppException : Exception {
    public ErrorCode ErrorCode { get; private set; }
    public string ErrorMessage { get; private set; }

    /// <summary>
    /// Initializes a new instance of the AppException class.
    /// </summary>
    /// <param name="message">The message value.</param>
    /// <param name="innerException">The innerException value.</param>
    public AppException(string message, Exception innerException) : base(message, innerException) {
        ErrorMessage = message;
    }

    /// <summary>
    /// Initializes a new instance of the AppException class.
    /// </summary>
    /// <param name="errorCode">The errorCode value.</param>
    /// <param name="innerException">The innerException value.</param>
    public AppException(ErrorCode errorCode, Exception innerException) : base(errorCode.ToString(), innerException) {
        ErrorCode = errorCode;
        ErrorMessage = errorCode.GetDescription();
    }

    /// <summary>
    /// Initializes a new instance of the AppException class.
    /// </summary>
    /// <param name="errorCode">The errorCode value.</param>
    /// <param name="extendedMessage">The extendedMessage value.</param>
    /// <param name="innerException">The innerException value.</param>
    public AppException(ErrorCode errorCode, string extendedMessage, Exception innerException) : base(errorCode.GetDescription(), innerException) {
        ErrorCode = errorCode;
        ErrorMessage = errorCode.GetDescription();
    }

    /// <summary>
    /// Executes Throw.
    /// </summary>
    /// <param name="errorCode">The errorCode value.</param>
    /// <param name="innerException">The innerException value.</param>
    public static void Throw(ErrorCode errorCode, Exception innerException) {
        throw new AppException(errorCode, innerException);
    }
}
