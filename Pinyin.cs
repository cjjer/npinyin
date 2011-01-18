/**
 * NPinyin包含一个公开类Pinyin，该类实现了取汉字文本首字母、文本对应拼音、以及
 * 获取和拼音对应的汉字列表等方法。由于汉字字库大，且多音字较多，因此本组中实现的
 * 拼音转换不一定和词语中的字的正确读音完全吻合。但绝大部分是正确的。
 * 
 * 最后感谢百度网友韦祎提供的常用汉字拼音对照表。见下载地址：
 * http://wenku.baidu.com/view/d725f4335a8102d276a22f46.html
 * 
 * 最后，我想简要地说明一下我的设计思路：
 * 首先，我将汉字按拼音分组后建立一个字符串数组（见PyCode.codes），然后使用程序
 * 将PyCode.codes中每一个汉字散列到和PyCode.codes相同长度的一个散列表中，（见
 * PyHash.hashes），当检索一个汉字的拼音时，首先从PyHash.hashes中获取和
 * 对应的PyCode.codes中数组下标，然后从对应字符串查找，当到要查找的字符时，字符
 * 串的前6个字符即包含了该字的拼音。
 * 
 * 此种方法的好处一是节约了存储空间，二是兼顾了查询效率。
 *
 * 如有意见，请与我联系反馈。我的邮箱是：qzyzwsy@gmail.com
 * 
 * 汪思言 2011年1月3日凌晨
 * */

using System;
using System.Collections.Generic;
using System.Text;

namespace NPinyin
{
  public static class Pinyin
  {
    // 返回中文文本的首字母
    public static string GetInitials(string text)
    {
      text = text.Trim();
      StringBuilder chars = new StringBuilder();
      for (var i = 0; i < text.Length; ++i)
      {
        string py = GetPinyin(text[i]);
        if (py != "") chars.Append(py[0]);
      }

      return chars.ToString().ToUpper();
    }

    // 返回中文文本的拼音
    public static string GetPinyin(string text)
    {
      text = text.Trim();
      StringBuilder sbPinyin = new StringBuilder();

      for (var i = 0; i < text.Length; ++i)
      {
        string py = GetPinyin(text[i]);
        if (py != "") sbPinyin.Append(py);
        sbPinyin.Append(" ");
      }

      return sbPinyin.ToString().Trim();
    }

    // 返回和拼音相同的汉字列表
    public static string GetChineseText(string Pinyin)
    {
      string key = Pinyin.Trim().ToLower();

      foreach (string str in PyCode.codes)
      {
        if (str.StartsWith(key + " ") || str.StartsWith(key + ":"))
          return str.Substring(7);
      }

      return "";
    }

    // 返回单个字符的汉字拼音
    public static string GetPinyin(char ch)
    {
      // 如果是英文字母则直接返回；
      string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
      if (letters.IndexOf(ch) != -1) return ch.ToString();

      // 如果是中文或数字，则从hash表中检索
      short hash = GetHashIndex(ch);
      for (var i = 0; i < PyHash.hashes[hash].Length; ++i)
      {
        short index = PyHash.hashes[hash][i];
        var pos = PyCode.codes[index].IndexOf(ch, 7);
        if (pos != -1)
          return PyCode.codes[index].Substring(0, 6).Trim();
      }

      return "";
    }

    private static short GetHashIndex(char ch)
    {
      return (short)((uint)ch % PyCode.codes.Length);
    }
  }
}
