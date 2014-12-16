using System.Collections.Generic;
using System.Linq;
using BaseLibS.Util;

namespace BaseLibS.Data{
	public class NeighbourList{
		private readonly Dictionary<int, HashSet<int>> neighborList = new Dictionary<int, HashSet<int>>();

		public void Add(int i, int j){
			Add2(i, j);
			Add2(j, i);
		}

		public bool IsEmptyAt(int i) { return !neighborList.ContainsKey(i); }

		public int[] GetClusterAt(int i){
			int[] cluster = GetClusterAtNoRemove(i);
			RemoveCluster(cluster);
			return cluster;
		}

		public int[][] GetAllClusters(){
			List<int[]> result = new List<int[]>();
			while (neighborList.Count > 0){
				result.Add(GetClusterAt(neighborList.Keys.First()));
			}
			return result.ToArray();
		}

		public void RemoveCluster(int[] cluster){
			foreach (int c in cluster.Where(c => neighborList.ContainsKey(c))){
				neighborList.Remove(c);
			}
		}

		public int[] GetClusterAtNoRemove(int i){
			HashSet<int> cluster = new HashSet<int>();
			Stack<int> todo = new Stack<int>();
			todo.Push(i);
			while (todo.Count > 0){
				int next = todo.Pop();
				if (!cluster.Contains(next)){
					if (neighborList.ContainsKey(next)){
						cluster.Add(next);
						foreach (int x in neighborList[next]){
							todo.Push(x);
						}
					}
				}
			}
			return ArrayUtils.ToArray(cluster);
		}

		private void Add2(int i, int j){
			if (!neighborList.ContainsKey(i)){
				neighborList.Add(i, new HashSet<int>());
			}
			neighborList[i].Add(j);
		}
	}
}