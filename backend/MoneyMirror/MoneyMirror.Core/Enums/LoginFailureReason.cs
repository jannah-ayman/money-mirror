namespace MoneyMirror.Core.Enums
{
    public enum LoginFailureReason
    {
        InvalidCredentials,
        EmailNotConfirmed,
        SoftDeletedRecoverable,
        PermanentlyDeleted
    }
}
