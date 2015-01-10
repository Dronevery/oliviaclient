using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using LitJson;
using System;
using System.Text;
using System.Threading;


public class  auth: MonoBehaviour 
{
	public static bool online = false;
	private connect Connect;
	public void begin()
	{
		try{
//			game0gui.gamelog("服务器连接成功，正在进行验证");	
			JsonData authmes = new JsonData();
			authmes["event"]="auth";
			authmes["username"]=publicvar.username;
			authmes["passwd"]=publicvar.passwd;
			Connect.send(authmes.ToJson());
		}
		catch
		{
//			game0gui.gamelog("认证失败");
		}
	}

	public auth(connect Connect)
	{
		this.Connect = Connect;
	}

	public void wait(string resp)
	{
		try
		{
			print(resp);
			JsonData data = JsonMapper.ToObject(resp);
			if( (string) data["type"]!="auth"  )
				return;
			string mes = (string) data["data"];
			if (string.Equals(mes,"WELCOME"))
			{
				begin();
			}
			if (string.Equals(mes,"CONFIRM") )
			{
//				game0gui.gamelog("认证成功");
//				publicvar.lengthmesh = (float) (double) data["lengthmesh"];
//				publicvar.loadwidth = (int) data["loadwidth"];
//				publicvar.basei = (int) data["basei"];
//				publicvar.basej = (int) data["basej"];

				JsonData authmes = new JsonData();
				authmes["type"]="status";
				authmes["status"]="ready";

				Connect.send(authmes.ToJson());
				online = true;
			}
		}
		catch (Exception e)
		{
			print("JSON Parse Failed\n"+e.ToString() );				
		}
	}
}
