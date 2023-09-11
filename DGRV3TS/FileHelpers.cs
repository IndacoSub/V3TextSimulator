using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGRV3TS
{
    public partial class FileManager
    {
        public int GetMaxLine()
        {

            switch (this.Type)
            {
                case LoadedFileType.Vo:
                    return this.VoList.Count;
                case LoadedFileType.Po:
                    return this.PoList.Count;
                case LoadedFileType.Txt:
                    return this.TxtList.Count;
                case LoadedFileType.Stx:
                    return this.StxFile.Sentences.Length;
                case LoadedFileType.Xlsx:
                    return this.XLSXList.Count;
                default:
                    return 0;
            }
        }

        public List<string> GetAllTranslatedText()
        {
            List<string> ret = new List<string>();

            switch (this.Type)
            {
                case LoadedFileType.Vo:
                    foreach (VoInternal vv in VoList)
                    {
                        // Default to first translation
                        ret.Add(vv.Translations[0]);
                    }
                    break;
                case LoadedFileType.Po:
                    foreach (PoInternal pp in PoList)
                    {
                        ret.Add(pp.MessageString);
                    }
                    break;
                case LoadedFileType.Txt:
                    foreach (TxtInternal tt in TxtList)
                    {
                        ret.Add(tt.Text);
                    }
                    break;
                case LoadedFileType.Stx:
                    foreach (string str in StxFile.Sentences)
                    {
                        ret.Add(str);
                    }
                    break;
                case LoadedFileType.Xlsx:
                    foreach (XLSXRow xlsx in XLSXList)
                    {
                        // Default to first translation
                        ret.Add(xlsx.Translations[0]);
                    }
                    break;
            }

            return ret;
        }

        public List<string> GetAllOriginalText()
        {
            List<string> ret = new List<string>();

            switch (this.Type)
            {
                case LoadedFileType.Vo:
                    foreach (VoInternal vv in VoList)
                    {
                        ret.Add(vv.OriginalMessage);
                    }
                    break;
                case LoadedFileType.Po:
                    foreach (PoInternal pp in PoList)
                    {
                        ret.Add(pp.OriginalMessage);
                    }
                    break;
                case LoadedFileType.Txt:
                    foreach (TxtInternal tt in TxtList)
                    {
                        ret.Add(tt.Text);
                    }
                    break;
                case LoadedFileType.Stx:
                    foreach (string str in StxFile.Sentences)
                    {
                        ret.Add(str);
                    }
                    break;
                case LoadedFileType.Xlsx:
                    foreach (XLSXRow xlsx in XLSXList)
                    {
                        ret.Add(xlsx.Original);
                    }
                    break;
            }

            return ret;
        }

        public string GetCurrentTranslation()
        {
            switch (this.Type)
            {
                case LoadedFileType.Vo:
                    // Default to first translation
                    return VoList[this.StringIndex].Translations[0];
                case LoadedFileType.Po:
                    return PoList[this.StringIndex].MessageString;
                case LoadedFileType.Txt:
                    return TxtList[this.StringIndex].Text;
                case LoadedFileType.Stx:
                    return StxFile.Sentences[this.StringIndex];
                case LoadedFileType.Xlsx:
                    // Default to first translation
                    return XLSXList[this.StringIndex].Translations[0];
                default:
                    return string.Empty;
            }
        }

        public string GetCurrentOriginal()
        {
            switch (this.Type)
            {
                case LoadedFileType.Vo:
                    return VoList[this.StringIndex].OriginalMessage;
                case LoadedFileType.Po:
                    return PoList[this.StringIndex].OriginalMessage;
                case LoadedFileType.Txt:
                    return TxtList[this.StringIndex].Text;
                case LoadedFileType.Stx:
                    return StxFile.Sentences[this.StringIndex];
                case LoadedFileType.Xlsx:
                    return XLSXList[this.StringIndex].Original;
                default:
                    return string.Empty;
            }
        }

        public List<string> GetAllSpeakers()
        {
            List<string> ret = new List<string>();

            switch(this.Type)
            {
                case LoadedFileType.Vo:
                    foreach(VoInternal vv in VoList)
                    {
                        ret.Add(vv.Character);
                    }
                    break;
                case LoadedFileType.Po:
                    foreach(PoInternal pp in PoList)
                    {
                        ret.Add(pp.Character);
                    }
                    break;
                case LoadedFileType.Stx:
                    if(StxFile.LoadedWRD != null)
                    {
                        int i = 0;
                        // TODO: Might be wrong

                        foreach(var ch in StxFile.LoadedWRD.charaNames)
                        {
                            if(i == ch.Key)
                            {
                                ret.Add(ch.Value);
                            } else
                            {
                                ret.Add("UnknownSpeaker");
                            }
                            i++;
                        }
                    }
                    break;
                default:
                    break;
            }

            return ret;
        }
    }
}
