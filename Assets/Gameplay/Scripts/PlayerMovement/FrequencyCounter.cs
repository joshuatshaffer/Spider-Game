using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrequencyCounter {
	private List<Rigidbody> things; 
	private List<int> frequencies;

	public FrequencyCounter () {
		things = new List<Rigidbody>();
		frequencies = new List<int>();
	}

	public void Occured (Rigidbody happend) {
		int i = things.IndexOf(happend);
		if (i >= 0) {
			++(frequencies[i]);
		} else {
			things.Add(happend);
			frequencies.Add(1);
		}
	}

	public Rigidbody Mode() {
		int iom = 0;
		for (int i=0; i<frequencies.Count; ++i) {
			if (frequencies[iom] < frequencies[i])
				iom = i;
		}
		return things[iom];
	}
}
