﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace liscuter
{
    internal class Program
    {
        private static string file;

        static void Main(string[] args)
        {
            string isFirstStart = ConfigurationManager.AppSettings["IsFirstStart"];
            int isFirstStartInt = Convert.ToInt32(isFirstStart);
            if (isFirstStartInt == 1)
            {
                Console.WriteLine("检测到你是第一次打开liscuter，先进行一下配置");
                Console.WriteLine("是(y)否(n)为追加(append)文本");
                string isAppend = Console.ReadLine();
                while (isAppend != "y" && isAppend != "n")
                {
                    Console.WriteLine("无效的数据结构，请重新输入");
                    Console.WriteLine("是(y)否(n)为追加(append)文本");
                    isAppend = Console.ReadLine();
                }
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings["IsAppend"].Value = isAppend;
                cfa.Save();
                cfa.AppSettings.Settings["IsFirstStart"].Value = "0";
                cfa.Save();
            }
            Console.WriteLine("欢迎使用听力裁剪器");
            Console.WriteLine("1.简介\n2.设置\n3.运行");
            Console.WriteLine("请输入序号：");
            string strNumber = Console.ReadLine();
            int number = Convert.ToInt32(strNumber);
            while (number != 1 && number != 2 && number != 3)
            {
                Console.WriteLine("无效的数字，请重新输入");
                Console.WriteLine("请输入序号：");
                strNumber = Console.ReadLine();
                number = Convert.ToInt32(strNumber);
            }
            if (number == 3)
            {
                if (System.IO.File.Exists("run.bat"))
                {
                    // Use a try block to catch IOExceptions, to
                    // handle the case of the file already being
                    // opened by another process.
                    // source2.mp3
                    try
                    {
                        System.IO.File.Delete("run.bat");
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                }
                if (System.IO.File.Exists("temp.mp3"))
                {
                    // Use a try block to catch IOExceptions, to
                    // handle the case of the file already being
                    // opened by another process.
                    // source2.mp3
                    try
                    {
                        System.IO.File.Delete("temp.mp3");
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                }
                Thread t = new Thread((ThreadStart)(() =>
                {
                    // 傻逼微软
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Multiselect = true;//该值确定是否可以选择多个文件
                    dialog.Title = "请选择文件夹";
                    dialog.Filter = "所有文件(*.*)|*.*";
                    //if (dialog.ShowDialog() == DialogResult.OK)
                    //{
                    //    file = dialog.FileName;
                    //}
                    while (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        dialog.Multiselect = true;//该值确定是否可以选择多个文件
                        dialog.Title = "请选择文件夹";
                        dialog.Filter = "所有文件(*.*)|*.*";
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            file = dialog.FileName;
                            break;
                        }
                        //string file = dialog.FileName;
                    }
                    file = dialog.FileName;

                }
));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();


                //OpenFileDialog dialog = new OpenFileDialog();
                //dialog.Multiselect = true;//该值确定是否可以选择多个文件
                //dialog.Title = "请选择文件夹";
                //dialog.Filter = "所有文件(*.*)|*.*";
                //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //{
                //    string file = dialog.FileName;
                //}

                //Console.WriteLine("请将音频文件放置根目录下，然后请将其重命名为\"source.mp3\"");
                //StreamWriterTwo streamWriterTwo = new StreamWriterTwo();


                //文件重命名复制
                string sourceFile = file;
                string destinationFile = System.IO.Directory.GetCurrentDirectory();
                destinationFile = System.IO.Path.Combine(destinationFile, "NoConvert.mp3");
                bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之 
                System.IO.File.Copy(sourceFile, destinationFile, isrewrite);

                string tcode = Transcoding.ToMP3("NoConvert");

                Console.WriteLine("请输入开始时间，单位：分:秒");
                string start = Console.ReadLine();
                Console.WriteLine("请输入结束时间，单位：分:秒");
                string end = Console.ReadLine();
                Console.WriteLine("请输入题号");
                string questionNumber = Console.ReadLine();
                string cut = $"ffmpeg -i temp.mp3  -vn -acodec copy -ss {start} -to {end} {questionNumber}.mp3";
                string general = tcode + cut;
                //streamWriterTwo.ExampleAsync(general);
                //String isAppend = ConfigurationManager.AppSettings["IsAppend"];
                //string temp;
                //if (isAppend == "y")
                //{
                //    temp = "true";
                //}
                //else
                //{
                //    temp = "false";
                //}
                //bool booltemp = Convert.ToBoolean(temp);
                //StreamWriter file = new StreamWriter("run.bat", append: booltemp);
                //await file.WriteLineAsync(general);
                StreamWriter sw = new StreamWriter("run.bat", true, System.Text.Encoding.Default);
                sw.WriteLine("@echo off");
                sw.WriteLine(tcode);//写入bat文件
                sw.WriteLine(cut);
                Console.WriteLine("是(y)否(n)继续切题");
                string isContinue = Console.ReadLine();
                while (isContinue != "y" && isContinue != "n")
                {
                    Console.WriteLine("无效的数据结构，请重新输入");
                    Console.WriteLine("是(y)否(n)继续切题");
                    isContinue = Console.ReadLine();
                }
                while (isContinue == "y")
                {
                    Console.WriteLine("请输入开始时间，单位：时:分:秒");
                    string Astart = Console.ReadLine();
                    Console.WriteLine("请输入结束时间，单位：时:分:秒");
                    string Aend = Console.ReadLine();
                    Console.WriteLine("请输入题号");
                    string AquestionNumber = Console.ReadLine();
                    string Acut = $"ffmpeg -i temp.mp3  -vn -acodec copy -ss {Astart} -to {Aend} {AquestionNumber}.mp3";
                    sw.WriteLine(Acut);
                    Console.WriteLine("是(y)否(n)继续切题");
                    isContinue = Console.ReadLine();
                }
                sw.WriteLine("Finish.vbs");
                sw.Flush();
                sw.Close();
                ThreadStart childref = new ThreadStart(RunMutithreadings.CallToChildThread);
                //Console.WriteLine("In Main: Creating the Child thread");
                Thread childThread = new Thread(childref);
                childThread.Start();
                //Console.ReadKey();

            }
        }
    }
}
