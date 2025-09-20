using Markdig;

namespace somi_thoughts
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string layout = File.ReadAllText("layout.html");
            string inputDir = "thoughts";
            string outputDir = "output";

            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);
            Directory.CreateDirectory(outputDir);
            Directory.CreateDirectory(Path.Combine(outputDir, "thoughts"));

            foreach (var pagePath in Directory.GetFiles(inputDir))
            {
                string thoughtId = Path.GetFileNameWithoutExtension(pagePath);
                var fileName = Path.GetFileNameWithoutExtension(pagePath) + ".html";
                var md = $"<div class=\"thought-title-holder\">Thought {thoughtId[..3]}: <span  class=\"thought-title\">{thoughtId[4..]}</span></div>"+ File.ReadAllText(pagePath);
                string finalHtml = SetContentMarkdown(layout, md);
                var outputPath = Path.Combine(outputDir, "thoughts", fileName);
                File.WriteAllText(outputPath, finalHtml);
            }

            string thoughsHtml = "";
            thoughsHtml += "<ul class=link-holder>";
            for (int i = 0; i < 1; i++)
            {
                foreach (var pagePath in Directory.GetFiles(inputDir).OrderDescending())
                {
                    string thoughtId = Path.GetFileNameWithoutExtension(pagePath);
                    thoughsHtml += $"<li><a href=\"thoughts/{thoughtId + ".html"}\" class=\"link\">{thoughtId.Replace("-", " – ")}</a></li>";
                }
            }

            var index = SetTag(layout, "content", thoughsHtml);
            thoughsHtml += "</ul>";
            File.WriteAllText(Path.Combine(outputDir, "index.html"), index);

            File.Copy("MozillaText-VariableFont_wght.ttf", $"{outputDir}/MozillaText-VariableFont_wght.ttf");
            var about = SetTag(layout, "content",
$@"
<p>
This website holds some raw unfiltered thoughts that I want to write down anonymously. They are written from me to <span class=""intro"">S o m i</span>. See them as writings from from a guy with a mild psychosis written to his crush that only exist in his mind.
</p>

<h2>
Anonymous
</h2>
<p>
Some of these thoughts are risky so just in case someone tries, the domain is bought from an anonymous domain registry using crypto I got from a shady crypto exchange. The email account is created using a new prepaid sim card. Everything from account creation and commits are done on a VM running Whonix. First time I actually needed privacy and it was quite a haste especially how each site blocks Tor activity. Although most of my thoughts are true, some identifying details are altered. Anyways I tried my best to stay hidden, I hope it stays that way.
</p>
<br/>
<h2>
Contact
</h2>
<p>
If you want to contact me you can email me at somi thoughts @ gmail . com (remove spaces) or via <a href=""https://github.com/somi-thoughts"">GitHub</a>. Be aware that I only sporadically log in. 
</p>
");
            File.WriteAllText(Path.Combine(outputDir, "about.html"), about);
            Console.WriteLine("Done!");
        }

        private static string SetContentMarkdown(string layout, string content)
        {
            var indexTag = layout.IndexOf("<!--[content]-->");
            string finalHtml = SetTag(layout, "content", " <div class=\"markdown\">" + Markdown.ToHtml(content) + " </div>");
            int end = finalHtml.IndexOf("Somi",indexTag, StringComparison.Ordinal) + "Somi".Length;
            int start = finalHtml[..end].LastIndexOf(">", StringComparison.Ordinal) + 1;
            var intro = finalHtml[start..end];
            finalHtml = finalHtml.Replace(intro, $"<span class=\"intro\">{intro}</span>");
            return finalHtml;
        }

        private static string SetTag(string layout, string tag, string content)
        {
            var finalHtml = layout.Replace($"<!--[{tag}]-->", content);
            return finalHtml;
        }
    }
}