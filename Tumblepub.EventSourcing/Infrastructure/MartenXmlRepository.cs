using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Xml.Linq;

namespace Tumblepub.EventSourcing.Infrastructure;

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
