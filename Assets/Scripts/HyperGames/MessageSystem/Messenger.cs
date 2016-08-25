using UnityEngine;
using System.Collections.Generic;

namespace HyperGames.MessageSystem {

    public interface Message {}

    public class Messenger {
        public delegate void MessageDelegate<T> (T msg) where T : Message;
        private delegate void MessageDelegate (Message msg, object filter);

        static private Dictionary<System.Type, MessageDelegate> delegates = new Dictionary<System.Type, MessageDelegate>();
        static private Dictionary<System.Delegate, MessageDelegate> delegateLookup = new Dictionary<System.Delegate, MessageDelegate>();

        static public void AddListener<T> (MessageDelegate<T> del, object filter = null) where T : Message {
            // Early-out if we've already registered this delegate
            if (delegateLookup.ContainsKey(del)) {
                return;
            }

			// Create a new non-generic delegate which calls our generic one.
			// This is the delegate we actually invoke.
			MessageDelegate internalDelegate;
			if (filter == null) {
				internalDelegate = (msg, fltr) => del((T)msg);
			} else {
				internalDelegate = (msg, fltr) => { if (fltr == filter) del((T)msg); };
			}

			delegateLookup[del] = internalDelegate;

            MessageDelegate tempDel;
            if (delegates.TryGetValue(typeof(T), out tempDel)) {
                delegates[typeof(T)] = tempDel + internalDelegate;
            } else {
                delegates[typeof(T)] = internalDelegate;
            }
        }

	    public static void ResetAllListeners() {
			delegates.Clear();
			delegateLookup.Clear();
        }

        static public void RemoveListener<T> (MessageDelegate<T> del) where T : Message {

            MessageDelegate internalDelegate;
            if (delegateLookup.TryGetValue(del, out internalDelegate)) {

                MessageDelegate tempDel;
                if (delegates.TryGetValue(typeof(T), out tempDel)) {

                    tempDel -= internalDelegate;
                    if (tempDel == null) {
                        delegates.Remove(typeof(T));
                    } else {
                        delegates[typeof(T)] = tempDel;
                    }
                }

                delegateLookup.Remove(del);
            }
        }

        static public void Dispatch(Message msg, object filter = null) {
			if(Debug.isDebugBuild)
			Profiler.BeginSample(msg.GetType().Name);

            MessageDelegate del;
            if (delegates.TryGetValue(msg.GetType(), out del)) {
                del.Invoke(msg, filter);
            }
			if (Debug.isDebugBuild)
				Profiler.EndSample();
        }
    }
}