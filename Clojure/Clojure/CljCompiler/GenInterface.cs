﻿/**
 *   Copyright (c) Rich Hickey. All rights reserved.
 *   The use and distribution terms for this software are covered by the
 *   Eclipse Public License 1.0 (http://opensource.org/licenses/eclipse-1.0.php)
 *   which can be found in the file epl-v10.html at the root of this distribution.
 *   By using this software in any fashion, you are agreeing to be bound by
 * 	 the terms of this license.
 *   You must not remove this notice, or any other, from this software.
 **/

/**
 *   Author: David Miller
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using clojure.lang.CljCompiler.Ast;

namespace clojure.lang
{
    public static class GenInterface
    {
        #region Factory

        public static Type GenerateInterface(string iName, ISeq extends, ISeq methods)
        {
            GenContext context;

            if (Compiler.IsCompiling)
            {
                //string path = (string)Compiler.COMPILE_PATH.deref();
                //if (path == null)
                //    throw new Exception("*compile-path* not set");
                //context = new GenContext(iName, ".dll", path, CompilerMode.File);
                context = (GenContext)Compiler.COMPILER_CONTEXT.deref();
            }
            else
                // TODO: In CLR4, should create a collectible type?
                //context = new GenContext(iName, ".dll", ".", CompilerMode.File);
                context = new GenContext(iName, ".dll", ".", AssemblyMode.Dynamic, FnMode.Full);

            Type[] interfaceTypes = GenClass.CreateTypeArray(extends == null ? null : extends.seq());

            TypeBuilder proxyTB = context.ModuleBuilder.DefineType(
                iName,
                TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract,
                null,
                interfaceTypes);


            DefineMethods(proxyTB, methods);

            Type t = proxyTB.CreateType();

            //if ( Compiler.IsCompiling )
            //    context.SaveAssembly();

            Compiler.RegisterDuplicateType(t);

            return t;
        }

        #endregion

        #region Defining methods

        private static void DefineMethods(TypeBuilder proxyTB, ISeq methods)
        {
            for (ISeq s = methods == null ? null : methods.seq(); s != null; s = s.next())
                DefineMethod(proxyTB, (IPersistentVector)s.first());
        }

        private static void DefineMethod(TypeBuilder proxyTB, IPersistentVector sig)
        {
            string mname = (string)sig.nth(0);
            Type[] paramTypes = GenClass.CreateTypeArray((ISeq)sig.nth(1));
            Type retType = (Type)sig.nth(2);

            MethodBuilder mb = proxyTB.DefineMethod(mname, MethodAttributes.Abstract | MethodAttributes.Public| MethodAttributes.Virtual, retType, paramTypes);
        }

        #endregion
    }
}
