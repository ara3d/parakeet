using System.Text;

namespace Ara3D.Parakeet.ConsoleApp;

/// <summary>
/// TODO: move this to Ara3D.Utils
/// </summary>
public class MarkdownBuilder
{
    public StringBuilder sb = new StringBuilder();

    public MarkdownBuilder AddLine(string text = "")
    {
        sb.AppendLine(text);
        return this;
    }

    public MarkdownBuilder AddLines(params string[] lines)
        => lines.Aggregate(this, (mb, line) => mb.AddLine(line));

    public MarkdownBuilder AddBold(string s)
        => AddString($"**{s}** ");

    public MarkdownBuilder AddItalic(string s)
        => AddString($"*{s}* ");

    public MarkdownBuilder AddListItem(string s)
        => AddLine($"- {s}");

    public MarkdownBuilder AddCodeBlock(string s)
        => AddLine("```").AddLine(s).AddLine("```");

    public MarkdownBuilder AddCode(string s)
        => AddString($"`{s}` ");

    public MarkdownBuilder AddHeader1(string text)
        => AddLine($"# {text}");

    public MarkdownBuilder AddHeader2(string text)
        => AddLine($"## {text}");

    public MarkdownBuilder AddHeader3(string text)
        => AddLine($"### {text}");

    public MarkdownBuilder AddLink(string text, string uri)
        => AddString($"[{text}]({uri}) ");

    public MarkdownBuilder AddString(string s)
    {
        sb.Append(s);
        return this;
    }
}