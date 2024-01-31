namespace Domain.Interface.Cryptography
{
    public interface ISha
    {
        string Encrypt(string dataToEncrypt);
    }
}
