////////////////////////////////////////////////////////////////////////////////////////////////////
// ExtractText.cs
// Copyright (c) 2018 Pdfix. All Rights Reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using PDFixSDK.Pdfix;
using System.IO;
using System.Collections.Generic;

namespace PDFix.App.Module
{
    class ExtractText
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // ParseText
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void ParseText(PdeText text)
        {
            string text_buffer = text.GetText();
            Console.WriteLine(text_buffer);
        }
        ///////////////////////////////////////////////////////////////////////
        // ParseElement
        ///////////////////////////////////////////////////////////////////////
        private static void ParseElement(PdeElement element , List<TextObject> lines)
        {
            // parse element based on type;
            PdfElementType elemType = element.GetType_();
            switch (elemType)
            {
                case PdfElementType.kPdeText:
                    var textObj = ((PdeText)element);
                    var line = new TextObject(textObj);
                    for (int i = 0; i < textObj.GetNumWords(); i++)
                    {
                        var word = textObj.GetWord(i);
                        line.words.Add(new TextObject(word));
                    }
                    lines.Add(line);
                    return;                    
            }

            int numChilds = element.GetNumChildren();
            for (int i = 0; i < numChilds; i++)
            {
                ParseElement(element.GetChild(i), lines);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        // ParsePage
        ///////////////////////////////////////////////////////////////////////
        private static void ParsePage(Pdfix pdfix, PdfPage page, List<TextObject> lines)
        {
            // get pageMap for the current page
            PdePageMap pageMap = page.AcquirePageMap(null, IntPtr.Zero);
            if (pageMap == null)
                throw new Exception(pdfix.GetError());

            // get page container
            PdeElement container = pageMap.GetElement();
            if (container == null)
                throw new Exception(pdfix.GetError());

            // parse children recursivelly
            ParseElement(container, lines);

            pageMap.Release();
        }

        public static void Run(
            String openPath                             // source PDF document
            )
        {
            Pdfix pdfix = new Pdfix();
            if (pdfix == null)
                throw new Exception("Pdfix initialization fail");

            PdfDoc doc = pdfix.OpenDoc(openPath, "");
            if (doc == null)
                throw new Exception(pdfix.GetError());
            var lines = new List<TextObject>();
            // iterate through pages and parse each page individually
            for (int i = 0; i < doc.GetNumPages(); i++)
            {
                PdfPage page = doc.AcquirePage(i);
                if (page == null)
                    throw new Exception(pdfix.GetError());
                ParsePage(pdfix, page, lines);
                page.Release();
            }

            doc.Close();
            pdfix.Destroy();
        }
    }
    public class FontColor
    {
        public FontColor(PDFixSDK.Pdfix.PdfRGB pdfRGB)
        {
            this.r = pdfRGB.r;
            this.g = pdfRGB.g;
            this.b = pdfRGB.b;
        }
        public int r;
        public int g;
        public int b;
    }
    public class ColorState
    {
        public ColorState(PDFixSDK.Pdfix.PdfColorState pdfColorState)
        {
            this.fillColor = new FontColor(pdfColorState.fill_color);
            this.fillOpacity = pdfColorState.fill_opacity;

            this.strokeColor = new FontColor(pdfColorState.stroke_color);
            this.strokeOpacity = pdfColorState.stroke_opacity;
        }
        public FontColor fillColor { get; set; }
        public double fillOpacity { get; set; }
        // public string fillType { get; set; }
        public FontColor strokeColor { get; set; }
        public double strokeOpacity { get; set; }
        //public string strokeType { get; set; }
    }
    public class Font
    {
        public Font(PDFixSDK.Pdfix.PdfFont pdfFont)
        {
            this.fontName = pdfFont.GetFontName();
            this.isBold = pdfFont.GetSystemFontBold();
            this.isItalic = pdfFont.GetSystemFontItalic();
        }
        public string fontName { get; set; }
        public bool isBold { get; set; }
        public bool isItalic { get; set; }
    }
    public class Rect
    {
        public Rect(PDFixSDK.Pdfix.PdfRect pdfRect)
        {
            this.left = pdfRect.left;
            this.top = pdfRect.top;
            this.right = pdfRect.right;
            this.bottom = pdfRect.bottom;
        }
        public double left { get; set; }
        public double top { get; set; }
        public double right { get; set; }
        public double bottom { get; set; }
    }
    public class TextObject
    {
        public TextObject(PDFixSDK.Pdfix.PdeText obj)
        {
            text = obj.GetText();
            lineSpacing = obj.GetLineSpacing();
            if (obj.HasTextState())
            {
                wordSpacing = obj.GetTextState().word_spacing;
                fontSize = obj.GetTextState().font_size;
                charSpacing = obj.GetTextState().char_spacing;
                font = new Font(obj.GetTextState().font);
                colorState = new ColorState(obj.GetTextState().color_state);
            }
            rect = new Rect(obj.GetBBox());
            if (words == null)
                words = new List<TextObject>();
        }
        public TextObject(PDFixSDK.Pdfix.PdeWord obj)
        {
            text = obj.GetText();
            if (obj.HasTextState())
            {
                wordSpacing = obj.GetTextState().word_spacing;
                fontSize = obj.GetTextState().font_size;
                charSpacing = obj.GetTextState().char_spacing;
                font = new Font(obj.GetTextState().font);
                colorState = new ColorState(obj.GetTextState().color_state);
            }
            rect = new Rect(obj.GetBBox());
        }
        public List<TextObject> words { get; set; }
        public string text { get; set; }
        public double wordSpacing { get; set; }
        public double lineSpacing { get; set; }
        public double charSpacing { get; set; }
        public double fontSize { get; set; }
        public Font font { get; set; }
        public ColorState colorState { get; set; }
        public Rect rect { get; set; }
    }
}