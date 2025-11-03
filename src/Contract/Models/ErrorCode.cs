namespace CleanArchitectureTest.Contract.Models
{
    public enum ErrorCode : short
    {
        ModelStateNotValid = 0,
        FieldDataInvalid = 1,
        NotFound = 2,
        AccessDenied = 3,
        ErrorInIdentity = 4,
        Exception = 5,
        Duplicated = 6,
        ContainChildren = 7

    }
}
