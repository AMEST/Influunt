using System.Collections.Generic;
using System.Linq;

namespace Influunt.Feed.Rss;
public static class UserAgents
{
        private static readonly IList<string> _userAgents = new List<string>
        {
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.101 YaBrowser/20.7.0.899 Yowser/2.5 Safari/537.36",
            "Mozilla/5.0 (X11; Linux i686; rv:139.0) Gecko/20100101 Firefox/139.0",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36 Edg/137.0.3296.52",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.105 Safari/537.36 Edge/18.19041",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 14.7; rv:139.0) Gecko/20100101 Firefox/139.0",
            "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:139.0) Gecko/20100101 Firefox/139.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 YaBrowser/25.4.1.1052 Yowser/2.5 Safari/537.36",
            "Mozilla/5.0 (Android 12; Mobile; rv:68.0) Gecko/68.0 Firefox/97.0",
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36",
            "Mozilla/5.0 (Linux; Android 9; Mi A3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.105 Safari/537.36",
            "Mozilla/5.0 (Linux; Android 10; SAMSUNG SM-N960F) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/10.1 Chrome/98.0.4758.105 Mobile Safari/537.36"
        };

        public static string Generate()
        {
            return _userAgents.OrderBy(q => System.Guid.NewGuid()).First();
        }
}
