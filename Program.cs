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
            thoughsHtml += "<ol reversed class=link-holder>";
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