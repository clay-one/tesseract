using Tesseract.Core.Index.Model;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Index
{
    public interface IAccountIndexMapper
    {
        AccountIndexModel MapAccountData(AccountData accountData);
    }
}