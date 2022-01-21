﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UnityEditor.XCodeEditor
{
	public partial class XCPlist : System.IDisposable
	{
		
		private string filePath;
		List<string> contents = new List<string>();
		public XCPlist(string fPath)
		{
			filePath = Path.Combine( fPath, "info.plist" );
			if( !System.IO.File.Exists( filePath ) ) {
				Debug.LogError( filePath +"路径下文件不存在" );
				return;
			}
			
			FileInfo projectFileInfo = new FileInfo( filePath );
			StreamReader sr = projectFileInfo.OpenText();
			while (sr.Peek() >= 0) 
			{
				contents.Add(sr.ReadLine());
			}
			sr.Close();
			
		}

		public XCPlist()
		{
			contents.Add ("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			contents.Add("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
		}

		public void AddLine(string value)
		{
			contents.Add (value);
		}
		
		public void AddKey(string key)
		{
			if(contents.Count < 2)
				return;
			contents.Insert(contents.Count - 2,key);
			
		}
		
		public void ReplaceKey(string key,string replace){
			for(int i = 0;i < contents.Count;i++){
				if(contents[i].IndexOf(key) != -1){
					contents[i] = contents[i].Replace(key,replace);
				}
			}
		}
		
		public void Save()
		{
			StreamWriter saveFile = File.CreateText(filePath);
			foreach(string line in contents)
				saveFile.WriteLine(line);
			saveFile.Close();
		}

		public void Save(string fPath)
		{
			StreamWriter saveFile = File.CreateText(fPath);
			foreach(string line in contents)
				saveFile.WriteLine(line);
			saveFile.Close();
		}
		
		public void Dispose()
		{
			
		}
	}
}