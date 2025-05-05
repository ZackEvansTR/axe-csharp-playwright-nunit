using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

using System.IO;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest : PageTest
{
    [Test]
    public async Task HasTitle()
    {
        string pageTitle = "Mars Commuter: Travel to Mars for Work or Pleasure!";
        await Page.GotoAsync("https://dequeuniversity.com/demo/mars/");
        await Expect(Page).ToHaveTitleAsync(new Regex(pageTitle));

        string workingDir = Environment.CurrentDirectory;
        string projectDir = Directory.GetParent(workingDir).Parent.Parent.FullName;
        string axeJsonDir = Path.Combine(projectDir, "axe-json");
        Directory.CreateDirectory(axeJsonDir);
        string axeResultsPath = Path.Combine(projectDir, "axe-json", "results.json");

        AxeResult axeResults = await Page!.RunAxe();

        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        string axeJson = JsonConvert.SerializeObject(axeResults, new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented
        });

        await File.WriteAllTextAsync(axeResultsPath, axeJson);
    }
}