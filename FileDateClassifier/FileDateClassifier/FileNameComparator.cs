using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDateClassifier
{
	public class FileNameComparator : IEqualityComparer<FileInfo>
	{
		public bool Equals(FileInfo x, FileInfo y)
		{
			return x.Name == y.Name;
		}

		public int GetHashCode(FileInfo obj)
		{
			return obj.Name.GetHashCode();
		}
	}
}
