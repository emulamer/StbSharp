﻿using System;
using System.IO;
using Sichem;

namespace StbSharp.StbTrueType.Generator
{
	class Program
	{
		static void Process()
		{
			var parameters = new ConversionParameters
			{
				InputPath = @"stb_truetype.h",
				ConversionMode = ConversionMode.SingleString,
				Defines = new[]
				{
						"STB_TRUETYPE_IMPLEMENTATION",
					},
				Namespace = "StbTrueTypeSharp",
				Class = "StbTrueType",
				SkipStructs = new string[]
				{
				},
				SkipGlobalVariables = new string[]
				{
				},
				SkipFunctions = new string[]
				{
						"stbtt__find_table",
				},
				Classes = new string[]
				{
						"stbtt_pack_context",
						"stbtt_fontinfo",
				},
				GlobalArrays = new string[]
				{
				},
				GenerateSafeCode = false,
			};

			var cp = new ClangParser();

			cp.Process(parameters);

			// Post processing
			Logger.Info("Post processing...");

			var data = cp.StringResult;
			data = Utility.ReplaceNativeCalls(data);

			data = data.Replace("(void *)(0)", "null");
			data = data.Replace("stbtt_vertex* vertices = 0;", "stbtt_vertex* vertices = null;");
			data = data.Replace("(flags & 16)?dx:-dx", "(flags & 16) != 0?dx:-dx");
			data = data.Replace("(flags & 32)?dy:-dy", "(flags & 32) != 0?dy:-dy");
			data = data.Replace("(vertices) == (0)", "vertices == null");
			data = data.Replace("sizeof((vertices[0]))", "sizeof(stbtt_vertex)");
			data = data.Replace("(int)(!(flags & 1))", "((flags & 1) != 0?0:1)");
			data = data.Replace("vertices = 0;", "vertices = null;");
			data = data.Replace("stbtt_vertex* comp_verts = 0;", "stbtt_vertex* comp_verts = null;");
			data = data.Replace("stbtt_vertex* tmp = 0;", "stbtt_vertex* tmp = null;");
			data = data.Replace(",)", ")");
			data = data.Replace("+ +", "+");
			data = data.Replace("(sizeof(stbtt__hheap_chunk) + size * count)",
				"((ulong)sizeof(stbtt__hheap_chunk)+ size * (ulong)(count))");
			data = data.Replace("size * hh->num_remaining_in_head_chunk",
				"size * (ulong)hh->num_remaining_in_head_chunk");
			data = data.Replace("sizeof((*z))", "sizeof(stbtt__active_edge)");
			data = data.Replace("_next_ = 0;", "_next_ = null;");
			data = data.Replace("sizeof((scanline[0]))", "sizeof(float)");
			data = data.Replace("int c = (int)(((a)->y0) < ((b)->y0));", "int c = (int)(a->y0 < b->y0?1:0);");
			data = data.Replace("sizeof((*e))", "sizeof(stbtt__edge)");
			data = data.Replace("sizeof((**contour_lengths))", "sizeof(int)");
			data = data.Replace("sizeof((points[0]))", "sizeof(stbtt__point)");
			data = data.Replace("sizeof((*context))", "sizeof(stbrp_context)");
			data = data.Replace("sizeof((*nodes))", "sizeof(stbrp_node)");
			data = data.Replace("sizeof((*rects))", "sizeof(stbrp_rect)");
			data = data.Replace("(int)(((a[0]) == (b[0])) && ((a[1]) == (b[1])));",
				"(int)(((a[0] == b[0]) && (a[1] == b[1]))?1:0);");

			File.WriteAllText(@"..\..\..\..\..\src\StbTrueTypeSharp\StbTrueType.Generated.cs", data);
		}

		static void Main(string[] args)
		{
			try
			{
				Process();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

			Console.WriteLine("Finished. Press any key to quit.");
			Console.ReadKey();
		}
	}
}