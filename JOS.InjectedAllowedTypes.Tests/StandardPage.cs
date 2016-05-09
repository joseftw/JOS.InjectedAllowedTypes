using System.Collections.Generic;
using EPiServer.Core;

namespace JOS.InjectedAllowedTypes.Tests
{
    public class StandardPage : PageData, IContainable
    {
        public IList<ContentReference> Items { get; set; }
    }
}
