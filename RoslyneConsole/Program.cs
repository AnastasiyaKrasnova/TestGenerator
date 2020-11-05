using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Immutable;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace RoslyneConsole
{
    class Program
    {

        public static void Main()
        {
            string path = @"C:\Users\home\Desktop\Labs\Третий Сем\СПП\OldTracer\OldTracer.sln";
            var msbuild = MSBuildWorkspace.Create();
            var solution = msbuild.OpenSolutionAsync(path).Result;
            ImmutableList<WorkspaceDiagnostic> diagnostics = msbuild.Diagnostics;
            foreach (var diagnostic in diagnostics)
            {
                Console.WriteLine(diagnostic.Message);
            }
            foreach (var project in solution.Projects)
            {
                foreach(var doc in project.Documents)
                {
                    Console.WriteLine(doc.Name+"  "+doc.FilePath);
                }
            }
            //Console.WriteLine(solution.ToString());
            /*var tree=CSharpSyntaxTree.ParseText(@"
            public partial class MyPartialClass
            {
                public void MyMethod()
                {
                    System.Console.WriteLine('Hello world');
                }
            }
            public partial class MyPartialClass
            {
            }
            ");

            var mscorelib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] { mscorelib }
                );

            var root = tree.GetRoot();
            var semanticModel = compilation.GetSemanticModel(tree);
            var methodSyntax = root.DescendantNodes().OfType<MethodDeclarationSyntax>().OfType<MethodDeclarationSyntax>()
           .Where(method =>
               method.Modifiers.Where(modifier =>
                   modifier.Kind() == SyntaxKind.PublicKeyword)
               .Any());
            foreach (var mths in methodSyntax)
            {
                var methodSymbol = semanticModel.GetDeclaredSymbol(mths);
                Console.WriteLine(methodSymbol);
            }

            
            var rewriter = new MyRewriter();
            var newroot = rewriter.Visit(root);
           var ifStatements = root.DescendantNodes().OfType<IfStatementSyntax>();
           foreach (var ifStm in ifStatements)
           {
               var body = ifStm.Statement;
               var block = SyntaxFactory.Block(body);
               var newIfStatement = ifStm.WithStatement(block);
               root = root.ReplaceNode(ifStm, newIfStatement);
           }
           Console.WriteLine(root.ToString());
           var diagnostics = tree.GetDiagnostics().Where(n => n.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).Any();
           var method= tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First();
           var returnType = SyntaxFactory.ParseTypeName("string");
           var newMethod = method.WithReturnType(returnType);*/
        }
    }

    /*public class MyRewriter : CSharpSyntaxRewriter
    {

        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            var body = node.Statement;
            var block = SyntaxFactory.Block(body);
            var newIfStatement = node.WithStatement(block);
            return newIfStatement;
        }
    } 

    public class MyWalker : CSharpSyntaxWalker
    {
        public MyWalker() : base(SyntaxWalkerDepth.Token)
        {

        }
        public StringBuilder sb = new StringBuilder();
        public override void VisitToken(SyntaxToken token)
        {
            sb.Append(token.ToFullString());
        }
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            sb.Append(node.ToFullString());
            base.VisitMethodDeclaration(node);
        }
    }*/
}
