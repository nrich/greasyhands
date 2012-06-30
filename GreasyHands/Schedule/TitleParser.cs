using System;
using System.Text.RegularExpressions;
using GreasyHands.DAL.Container;

namespace GreasyHands.Schedule
{
    public class TitleParser
    {
        
        private const string CONTINUING_TITLE_PATTERN = @"^(.+?)\s+#(\d+)\s*(.*?)$";
        private const string LIMITED_TITLE_PATTERN = @"^(.+?)\s+#(\d+)\s+\([O0][Ff]\s+(\d+)\)\s*(.*?)$";
        private const string ROMAN_NUMERALS = @"\bM{0,4}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})\b";

        private readonly Func<TitleInfo> titleInfoFunc;

        public TitleParser(Func<TitleInfo> titleInfoFunc)
        {
            this.titleInfoFunc = titleInfoFunc;
        }

        public bool MatchRemove(ref string text, string pattern)
        {
            var r = new Regex(pattern);

            var transformed = r.Replace(text, "");

            var res = transformed.Length != text.Length;

            text = transformed;

            return res;
        }

        public bool IsPublisher(string line)
        {
            return Publisher.IsPublisher(line);
        }

        public bool IsBadTitle(string title)
        {
            var df = new Regex(@"^DF\s+");
            var pp = new Regex(@"\(PP #\d+\)");

            return df.IsMatch(title) || pp.IsMatch(title);
        }

        public bool IsLimitedTitle(string title)
        {
            var regex = new Regex(LIMITED_TITLE_PATTERN);

            return regex.Match(title).Success;
        }

        public bool IsContinuingTitle(string title)
        {
            if (IsLimitedTitle(title))
                return false;

            var regex = new Regex(CONTINUING_TITLE_PATTERN);

            return regex.Match(title).Success;
        }

        public TitleInfo BuildTitleInfo(string title, string publisher, DateTime shippingDate)
        {
			if (IsBadTitle(title))
				return null;

            TitleInfo titleInfo = null;
            if (IsLimitedTitle(title))
            {
                titleInfo = GetLimitedTitleName(title);
            }

            if (IsContinuingTitle(title))
            {
                titleInfo = GetContinuingTitleName(title);
            }

            if (titleInfo != null)
            {
                titleInfo.Publisher = publisher;
                titleInfo.Released = shippingDate;

                Console.WriteLine(titleInfo);
            }

            return titleInfo;
        }

        private string upper(Match m)
        {
            var text = m.Value;

            return text.ToUpper();
        }

        private string upperSecond(Match m)
        {
            var first = m.Groups[1].Value;
            var second = m.Groups[2].Value;

            return first + second.ToUpper();
        }

        private string lowerUpperFirst(Match m)
        {
            var text = m.Value;
            text = text.Substring(0, 1) + text.Substring(1).ToLower();

            return text;
        }

        public string FixTitle(string title)
        {
            return NormaliseName(title.ToUpper());
        }

        private string NormaliseName(string title)
        {

            title = title.Replace("O/T", "OF THE");
            title = title.Replace(" OT ", " OF THE ");
            title = title.Replace("I/T", "IN THE");
            title = title.Replace("BTVS", "BUFFY THE VAMPIRE SLAYER");

            {
                var r = new Regex(@"([^\s]+)");
                var myEvaluator = new MatchEvaluator(lowerUpperFirst);

                title = r.Replace(title, myEvaluator);
            }

            {
                var r = new Regex(@"(J[ls]a|Tv|Dvd|Sfx|\(\w|Gi\s|Dc\s|Idw|Usa|Sg\s)");
                var myEvaluator = new MatchEvaluator(upper);

                title = r.Replace(title, myEvaluator);
            }

            {
                var r = new Regex(ROMAN_NUMERALS, RegexOptions.IgnoreCase);
                var myEvaluator = new MatchEvaluator(upper);

                title = r.Replace(title, myEvaluator);
            }

            {
                var r = new Regex(@"(\w\-)(\w)");
                var myEvaluator = new MatchEvaluator(upperSecond);

                title = r.Replace(title, myEvaluator);
            }

            {
                var r = new Regex(@"\s+");
                title = r.Replace(title, " ");
            }

            return title;
        }

        private TitleInfo GetContinuingTitleName(string title)
        {
            var titleInfo = CreateTitleInfo(title);
            var regex = new Regex(CONTINUING_TITLE_PATTERN);

            var match = regex.Match(titleInfo.Name);

            if (!match.Success)
                throw new ArgumentException("Title does not match continuing pattern");

            titleInfo.Name = NormaliseName(match.Groups[1].Value);
            titleInfo.Num = int.Parse(match.Groups[2].Value);
            titleInfo.Variant = match.Groups[3].Value;

            return titleInfo;
        }
        
        private TitleInfo GetLimitedTitleName(string title)
        {
            var titleInfo = CreateTitleInfo(title);
            var regex = new Regex(LIMITED_TITLE_PATTERN);

            var match = regex.Match(titleInfo.Name);

            if (!match.Success)
                throw new ArgumentException("Title does not match limited pattern");

            titleInfo.Name = NormaliseName(match.Groups[1].Value);
            titleInfo.Num = int.Parse(match.Groups[2].Value);
            titleInfo.Limited = int.Parse(match.Groups[3].Value);
            titleInfo.Variant = match.Groups[4].Value;

            return titleInfo;
        }

        private TitleInfo CreateTitleInfo(string title)
        {
            var titleInfo = titleInfoFunc();

            var fragments = title.Split('\t');
            string code = fragments[0];
            title = fragments[1];

            titleInfo.Code = code;
            titleInfo.Mature = MatchRemove(ref title, @"\s*\(MR\)");
            titleInfo.OneShot = MatchRemove(ref title, @"\s*\(?ONE SHOT\)?");
            titleInfo.Trade = MatchRemove(ref title, @"\s*\bTP\b");
            titleInfo.GraphicNovel = MatchRemove(ref title, @"\s+GN");
            titleInfo.HardCover = MatchRemove(ref title, @"\s+HC");
            titleInfo.Name = title;

            return titleInfo;
        }
    }
}
