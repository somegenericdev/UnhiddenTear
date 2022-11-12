namespace UnhiddenTear
{
    public class EncryptedFileDTO
    {
        public byte[] encryptedData;
        public byte[] IV;

        public EncryptedFileDTO(byte[] _encryptedData, byte[] _IV)
        {
            encryptedData = _encryptedData;
            IV = _IV;
        }
    }
}