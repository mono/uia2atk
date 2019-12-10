using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Mono.UIAutomation.Winforms.Navigation
{
	internal partial class Navigation
	{
		internal class Chain
		{
			public static void LinkOrderedChains (Chain chain1, Chain chain2)
			{
				chain1.NextChain = chain2;
				chain2.PrevChain = chain1;
			}

			public Chain ()
			{
				Clear ();
			}

			public void SafeInsertAt (int index, FragmentControlProvider newChild)
			{
				if (Count == 0 || index < 0 || index >= Count)
					AppendToEnd (newChild);
				else 
					InsertBefore (newChild, GetByIndex (index));
			}

			public void InsertBefore (FragmentControlProvider newChild, FragmentControlProvider baseChild)
			{
				LinkOrdered (baseChild.Navigation.PreviousProvider, newChild);
				LinkOrdered (newChild, baseChild);
				++Count;

				if (baseChild == First)
					SetAsFirst (newChild);
			}

			public void AppendToEnd (FragmentControlProvider newChild)
			{
				LinkOrdered (Last, newChild);
				++Count;

				SetAsLast (newChild);
				if (Count == 1)
					SetAsFirst (newChild);
			}

			public void Remove (FragmentControlProvider child)
			{
				if (child == null)
					throw new ArgumentNullException ();

				LinkOrdered (child.Navigation.PreviousProvider, child.Navigation.NextProvider);
				--Count;

				if (Count == 0)
					SetLinksForEmptyCase ();
				else if (child == First)
					SetAsFirst (child.Navigation.NextProvider);
				else if (child == Last)
					SetAsLast (child.Navigation.PreviousProvider);
			}

			public FragmentControlProvider GetByIndex (int index)
			{
				var p = First;
				for (var i = 0; i < index; ++i)
					p = p.Navigation.NextProvider;
				return p;
			}

			public FragmentControlProvider TryGetByComponentOfProvider (Component childComponent)
			{
				if (childComponent == null)
					return null;
				if (First == null)
					return null;

				var p = First;
				while (true)
				{
					if (childComponent.Equals (p.Component))
						return p;
					if (p == Last)
						return null;
					p = p.Navigation.NextProvider;
				}
			}

			public int TryIndexOf (FragmentControlProvider child)
			{
				const int ERR = -1;

				if (child == null)
					return ERR;
				if (First == null)
					return ERR;

				var p = First;
				for (var index = 0; ; ++index)
				{
					if (p.Equals (child))
						return index;
					if (p == Last)
						return ERR;
					p = p.Navigation.NextProvider;
				}
			}

			public List<FragmentControlProvider> ToList ()
			{
				var list = new List<FragmentControlProvider> ();
				if (First == null)
					return list;

				var p = First;
				while (true)
				{
					list.Add (p);
					if (p == Last)
						break;
					p = p.Navigation.NextProvider;
				}
				return list;
			}

			public void Clear ()
			{
				First = null;
				Last = null;
				Count = 0;
				_prevChain = null;
				_nextChain = null;
			}

			public Chain PrevChain
			{
				get { return _prevChain; }
				protected set
				{
					_prevChain = value;
					if (Count == 0)
						SetLinksForEmptyCase ();
					else
						SetAsFirst (First);
				}
			}

			public Chain NextChain
			{
				get { return _nextChain; }
				protected set
				{
					_nextChain = value;
					if (Count == 0)
						SetLinksForEmptyCase ();
					else
						SetAsLast (Last);
				}
			}

			private void SetLinksForEmptyCase ()
			{
				First = null;
				Last = null;

				if (NextChain?.First != null)
					NextChain.First.Navigation.PreviousProvider = PrevChain?.Last;
				if (PrevChain?.Last != null)
					PrevChain.Last.Navigation.NextProvider = NextChain?.First;
			}

			private void SetAsLast (FragmentControlProvider child)
			{
				Last = child;
				Last.Navigation.NextProvider = NextChain?.First;
				if (NextChain?.First != null)
					NextChain.First.Navigation.PreviousProvider = Last;
			}

			private void SetAsFirst (FragmentControlProvider child)
			{
				if (null == child)
					throw new Exception($"child==null Count={Count}");
				if (null == child.Navigation)
					throw new Exception("child.Navigation==null");
				First = child;
				First.Navigation.PreviousProvider = PrevChain?.Last;
				if (PrevChain?.Last != null)
					PrevChain.Last.Navigation.NextProvider = First;
			}

			private static void LinkOrdered (FragmentControlProvider p1, FragmentControlProvider p2)
			{
				if (p1 != null)
					p1.Navigation.NextProvider = p2;
				if (p2 != null)
					p2.Navigation.PreviousProvider = p1;
			}

			private FragmentControlProvider _first;
			private FragmentControlProvider _last;
			public FragmentControlProvider First { get; private set; }
			public FragmentControlProvider Last { get; private set; }
			public int Count { get; private set; }

			private Chain _prevChain;
			private Chain _nextChain;
		}
	}
}