using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TPresenter.Library.Win32;

namespace TPresenter.Utils
{
    public delegate void ActionRef<T>(ref T item);

    public static class MessageLoop
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public static Dictionary<uint, ActionRef<Message>> messageDictionary;
        public static Queue<Message> messageQueue;
        public static Queue<WinApi.CopyData> messageCopyDataQueue;
        public static List<Message> tmpMessages;
        public static List<WinApi.CopyData> tmpCopyData;

        static MessageLoop()
        {
            const int initialQueueCapasity = 64;
            messageDictionary = new Dictionary<uint, ActionRef<Message>>();
            messageQueue = new Queue<Message>(initialQueueCapasity);
            messageCopyDataQueue = new Queue<WinApi.CopyData>(initialQueueCapasity);
            tmpMessages = new List<Message>(initialQueueCapasity);
            tmpCopyData = new List<WinApi.CopyData>(initialQueueCapasity);
        }

        public static void Process()
        {
            lock (messageQueue)
            {
                tmpMessages.AddRange(messageQueue);
                tmpCopyData.AddRange(messageCopyDataQueue);
                messageQueue.Clear();
                messageCopyDataQueue.Clear();
            }

            int tmpCopyDataIndex = 0;
            for (int ind = 0; ind < tmpMessages.Count; ind++)
            {
                var msg = tmpMessages[ind];
                if (msg.Msg != WMCodes.COPYDATA)
                    ProcessMessage(ref msg);
                else if (tmpCopyDataIndex < tmpCopyData.Count)
                {
                    var copyDataStruct = tmpCopyData[tmpCopyDataIndex++];
                    unsafe
                    {
                        void* ptr = &copyDataStruct;
                        msg.LParam = (IntPtr)ptr;
                        ProcessMessage(ref msg);
                    }
                    Marshal.FreeHGlobal(copyDataStruct.DataPointer);
                }
            }

            tmpMessages.Clear();
            tmpCopyData.Clear();
        }

        public static void AddMessageHandler(uint wmCode, ActionRef<Message> messageHandler)
        {
            if (messageDictionary.ContainsKey(wmCode))
                messageDictionary[wmCode] += messageHandler;
            else
                messageDictionary.Add(wmCode, messageHandler);
        }

        public static void AddMessageHandler(WinApi.WM wmCode, ActionRef<Message> messageHandler)
        {
            AddMessageHandler((uint)wmCode, messageHandler);
        }

        public static void RemoveMessageHandler(uint wmCode, ActionRef<Message> messageHandler)
        {
            if (messageDictionary.ContainsKey(wmCode))
                messageDictionary[wmCode] -= messageHandler;
        }

        public static void RemoveMessageHandler(WinApi.WM wmCode, ActionRef<Message> messageHandler)
        {
            RemoveMessageHandler((uint)wmCode, messageHandler);
        }

        public static void AddMessage(ref Message message)
        {
            lock (messageQueue)
            {
                if(message.Msg == WMCodes.COPYDATA)
                {
                    WinApi.CopyData copyData = (WinApi.CopyData)message.GetLParam(typeof(WinApi.CopyData));
                    IntPtr dataPointerCopy = Marshal.AllocHGlobal(copyData.DataSize);
                    CopyMemory(dataPointerCopy, copyData.DataPointer, (uint)copyData.DataSize);
                    copyData.DataPointer = dataPointerCopy;
                    messageCopyDataQueue.Enqueue(copyData);
                }
                messageQueue.Enqueue(message);
            }
        }

        public static void ClearMessageQueue()
        {
            lock (messageQueue)
                messageQueue.Clear();
        }

        public static void ProcessMessage(ref Message message)
        {
            ActionRef<Message> output = null;
            messageDictionary.TryGetValue((uint)message.Msg, out output);
            output?.Invoke(ref message);
        }
    }
}
