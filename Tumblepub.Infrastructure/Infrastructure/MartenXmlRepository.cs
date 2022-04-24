using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace Tumblepub.Infrastructure.Infrastructure;

public class MartenXmlRepository : IXmlRepository
{
    public IReadOnlyCollection<XElement> GetAllElements()
    {
        throw new NotImplementedException();
    }

    public void StoreElement(XElement element, string friendlyName)
    {
        throw new NotImplementedException();
    }
}
