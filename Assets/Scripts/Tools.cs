using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

public static class Tools
{
	public static Person GetNearest(Person myself, IEnumerable<Person> persons) {
		if(!persons.Any()) {
			return null;
		}
		return persons.MinBy(x => (x.transform.position - myself.transform.position).sqrMagnitude);
	}
	
	public static float Distance(Person x, Person y) {
		return (x.transform.position - y.transform.position).magnitude;
	}
	
	public static float DistanceSquare(Person x, Person y) {
		return (x.transform.position - y.transform.position).sqrMagnitude;
	}
	
}

