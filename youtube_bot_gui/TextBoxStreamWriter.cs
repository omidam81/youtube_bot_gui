using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace youtube_bot_gui
{
    class TextBoxStreamWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
        }
        static Semaphore consoleSemaphore = new Semaphore(1,1);
        public override void Write(char value)
        {
            consoleSemaphore.WaitOne();
            base.Write(value);
            if (_output.InvokeRequired)
            {
                _output.Invoke(new MethodInvoker(delegate { _output.AppendText(value.ToString()); }));
            }

            //_output.AppendText(value.ToString()); // When character data is written, append it to the text box.
            consoleSemaphore.Release();
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}