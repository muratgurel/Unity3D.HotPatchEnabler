﻿using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace hotpatch
{
	[TestFixture]
	class TestHotPatch
	{
		Type MethodsToBePatchedType;
		Type ClassToBePatchedType;

		[OneTimeSetUp]
		public void TestActivePatch()
		{
			HotPatchEditor.Active(new[] { "ToBePatched.dll" }, (name) => name + ".mod.dll", processSymbols: true);
			var patched = Assembly.LoadFile(
				Path.Combine(
					Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
					"ToBePatched.dll.mod.dll"));
			MethodsToBePatchedType = patched.GetType("ToBePatched.MethodsToBePatched");
			ClassToBePatchedType = patched.GetType("ToBePatched.ClassToBePatched");

			var stub = patched.GetType("ToBePatched.PatchStub");
			stub.InvokeMember("Apply", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, new object[] { });
		}

		[Test]
		public void TestMethodsToBePatchedType_PublicFunc()
		{
			var inst = Activator.CreateInstance(MethodsToBePatchedType);
			MethodsToBePatchedType.InvokeMember("PublicFunc", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, inst, new object[] { });
		}


		[Test]
		public void TestMethodsToBePatchedType_PrivateFunc()
		{
			var inst = Activator.CreateInstance(MethodsToBePatchedType);
			MethodsToBePatchedType.InvokeMember("PrivateFunc", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic, null, inst, new object[] { });
		}

		[Test]
		public void TestMethodsToBePatchedType_PrivateFuncWithRet()
		{
			var inst = Activator.CreateInstance(MethodsToBePatchedType);
			var ret = (int)MethodsToBePatchedType.InvokeMember("PriviateFuncWithRet", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic, null, inst, new object[] { });
			Assert.AreEqual(10, ret);
		}

		[Test]
		public void TestMethodsToBePatchedType_PublicFuncWithRet()
		{
			var inst = Activator.CreateInstance(MethodsToBePatchedType);
			var ret = (int)MethodsToBePatchedType.InvokeMember("PublicFuncWithRet", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, inst, new object[] { });
			Assert.AreEqual(20, ret);
		}

		[Test]
		public void TestMethodsToBePatchedType_PublicFuncWithOutAndRef()
		{
			var inst = Activator.CreateInstance(MethodsToBePatchedType);
			var args = new object[] { 10, null };
			var ret = (int)MethodsToBePatchedType.InvokeMember("PublicFuncWithOutAndRef", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, inst, args);
			Assert.AreEqual(10, args[0]);
			Assert.AreEqual(20, args[1]);
			Assert.AreEqual(30, ret);
		}

		[Test]
		public void TestMethodsToBePatchedType_StaticFuncWithParam()
		{
			var inst = Activator.CreateInstance(MethodsToBePatchedType);
			var args = new object[] { 12345 };
			var ret = (int)MethodsToBePatchedType.InvokeMember("StaticFuncWithParam", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, inst, args);
			Assert.AreEqual(12345, ret);
		}



	}
}
