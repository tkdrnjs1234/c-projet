using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;
using System.Threading;

namespace MDD_Text
{
    public partial class Form2 : Form
    {
        private int docCount = 1;
        private Thread clockThread;

        public Form2()
        {
            InitializeComponent();
            IsMdiContainer = true;

            toolStripStatusLabel3.Text = "준비 완료";

            clockThread = new Thread(UpdateClock);
            clockThread.IsBackground = true;
            clockThread.Start();
        }

        private void UpdateClock()
        {
            while (true)
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        toolStripStatusLabel1.Text =
                            DateTime.Now.ToString("yyyy-MM-dd");

                        toolStripStatusLabel2.Text =
                            DateTime.Now.ToString("tt hh:mm:ss");
                    }));
                }

                Thread.Sleep(1000);
            }
        }

        private void NewDocument() //새로만들기
        {
            Form3 doc = new Form3();

            doc.MdiParent = this;

            doc.Text = $"문서 {docCount++}";

            doc.Show();

            toolStripStatusLabel3.Text = "새 문서 생성";
        }

        private Form3? GetActiveDocument()
        {
            return this.ActiveMdiChild as Form3;
        }

        private void OpenDocument()  // 파일 열기
        {
            openFileDialog1.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            openFileDialog1.Title = "텍스트 파일 열기";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Form3 doc = new Form3();

                doc.MdiParent = this;

                doc.Editor.Text =
                    File.ReadAllText(openFileDialog1.FileName);

                doc.FilePath = openFileDialog1.FileName;

                doc.Text =
                    Path.GetFileName(openFileDialog1.FileName);


                doc.Show();

                toolStripStatusLabel3.Text = "파일 열기 완료";
            }
        }

        private void CloseDocument() //닫기
        {
            Form3? doc = GetActiveDocument();

            if (doc == null)
            {
                MessageBox.Show(
                    "열려있는 문서가 없습니다.",
                    "알림",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            doc.Close();
            toolStripStatusLabel3.Text = "문서 닫기 완료";
        }

        private void CloseAllDocuments()  //모두 닫기
        {
            if (this.MdiChildren.Length == 0)
            {
                MessageBox.Show(
                    "열려있는 문서가 없습니다.",
                    "알림",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            foreach (Form child in this.MdiChildren)
            {
                child.Close();
            }
            toolStripStatusLabel3.Text = "모든 문서 닫기 완료";
        }


        private void SaveDocument() //저장
        {
            Form3? doc = GetActiveDocument();

            if (doc == null)
            {
                MessageBox.Show(
                    "저장할 문서가 없습니다.",
                    "저장",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (string.IsNullOrEmpty(doc.FilePath))
            {
                SaveAsDocument();
                return;
            }

            File.WriteAllText(
                doc.FilePath,
                doc.Editor.Text
            );

            toolStripStatusLabel3.Text = "저장완료";
        }


        private void SaveAsDocument() //다른이름으로 저장
        {
            Form3? doc = GetActiveDocument();

            if (doc == null)
            {
                MessageBox.Show(
                    "저장할 문서가 없습니다.",
                    "저장",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            saveFileDialog1.Filter = "텍스트 파일 (*.txt)|*.txt";
            saveFileDialog1.Title = "텍스트 파일 저장";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(
                    saveFileDialog1.FileName,
                    doc.Editor.Text
                );

                doc.FilePath = saveFileDialog1.FileName;
                doc.Text = Path.GetFileName(saveFileDialog1.FileName);
            }

            toolStripStatusLabel3.Text = "다른 이름으로 저장";
        }

        private void SetupPage() //페이지 설정
        {
            pageSetupDialog1.Document = printDocument1;
            pageSetupDialog1.ShowDialog();
            toolStripStatusLabel3.Text = "페이지 설정";
        }

        private void PrintDocumentFile() //출력
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
            toolStripStatusLabel3.Text = "출력";
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)//위에 출력기능은 프린터한테 출력 해! 그리고 프린터가 어떤걸 출력하는지 알려주는 것이 이 코드

        {
            Form3? doc = GetActiveDocument();

            if (doc == null)
            {
                MessageBox.Show(
                    "출력할 문서가 없습니다.",
                    "출력",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            e.Graphics!.DrawString(
                doc.Editor.Text,
                doc.Editor.Font,
                Brushes.Black,
                100,
                100
            );
        }

        private void PreviewPrint() //미리보기
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();

            toolStripStatusLabel3.Text = "출력 미리보기";
        }

        private void ExitProgram() //끝내기
        {
            Application.Exit();
        }

        private void UndoText()//지우기 취소(방금한 작업취소)
        {
            Form3? doc = GetActiveDocument();

            if (doc == null)
            {
                MessageBox.Show(
                    "편집할 문서가 없습니다.",
                    "편집",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (doc.Editor.CanUndo)
            {
                doc.Editor.Undo();
            }
            toolStripStatusLabel3.Text = "작업취소";
        }

        private void CutText() // 잘라내기
        {
            Form3? doc = GetActiveDocument();

            if (doc == null)
            {
                MessageBox.Show(
                    "편집할 문서가 없습니다.",
                    "편집",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (doc.Editor.SelectedText != "")
            {
                doc.Editor.Cut();
            }
            toolStripStatusLabel3.Text = "잘라내기";
        }

        private void CopyText() //복사
        {
            Form3? doc = GetActiveDocument();

            if (doc == null)
            {
                MessageBox.Show(
                    "편집할 문서가 없습니다.",
                    "편집",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (doc.Editor.SelectedText != "")
            {
                doc.Editor.Copy();
            }
            toolStripStatusLabel3.Text = "복사";
        }

        private void PasteText() //붙여넣기
        {
            Form3? doc = GetActiveDocument();

            if (doc == null)
            {
                MessageBox.Show(
                    "편집할 문서가 없습니다.",
                    "편집",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            doc.Editor.Paste();
            toolStripStatusLabel3.Text = "붙여넣기";
        }

        private void ChangeFont() //글꼴 변경
        {
            Form3? doc = GetActiveDocument();

            if (doc == null)
            {
                MessageBox.Show(
                    "서식을 적용할 문서가 없습니다.",
                    "서식",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;

            }

            fontDialog1.Font = doc.Editor.Font;

            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                doc.Editor.Font = fontDialog1.Font;
            }
            toolStripStatusLabel3.Text = "글꼴 변경완료";
        }

        private void ChangeBackColor() //배경색 변경
        {
            Form3? doc = GetActiveDocument();

            if (doc == null)
            {
                MessageBox.Show(
                    "서식을 적용할 문서가 없습니다.",
                    "서식",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            colorDialog1.Color = doc.Editor.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                doc.Editor.BackColor = colorDialog1.Color;
            }
            toolStripStatusLabel3.Text = "배경색 변경완료";
        }

        private void CascadeWindows() //계단식 정렬
        {
            if (this.MdiChildren.Length == 0)
            {
                MessageBox.Show(
                    "정렬할 문서가 없습니다.",
                    "창 정렬",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            LayoutMdi(MdiLayout.Cascade);
            toolStripStatusLabel3.Text = "계단식 정렬";
        }

        private void TileWindows() //바둑판식 정렬
        {
            if (this.MdiChildren.Length == 0)
            {
                MessageBox.Show(
                    "정렬할 문서가 없습니다.",
                    "창 정렬",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            LayoutMdi(MdiLayout.TileHorizontal);
            toolStripStatusLabel3.Text = "바둑판식 정렬";
        }

        private void ArrangeIcons() //아이콘식 정렬
        {
            if (this.MdiChildren.Length == 0)
            {
                MessageBox.Show(
                    "정렬할 문서가 없습니다.",
                    "창 정렬",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            LayoutMdi(MdiLayout.ArrangeIcons);
            toolStripStatusLabel3.Text = "아이콘 정렬";
        }

        private void 새로만들기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDocument();
        }

        private void 열기OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void 저장SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDocument();
        }

        private void 다른이름으로저장AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsDocument();
        }

        private void 닫기CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseDocument();
        }

        private void 모두닫기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllDocuments();
        }

        private void 페이지설정UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetupPage();
        }

        private void 출력PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDocumentFile();
        }

        private void 미리보기RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviewPrint();
        }

        private void 끝내기XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitProgram();
        }

        private void 지우기취소ZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoText();
        }

        private void 잘라내기XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutText();
        }

        private void 복사CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyText();
        }

        private void 붙여넣기VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteText();
        }

        private void 글꼴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeFont();
        }

        private void 배경색BToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBackColor();
        }

        private void 계단식정렬ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CascadeWindows();
        }

        private void 바둑판식정렬ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TileWindows();
        }

        private void 아이콘식정렬ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrangeIcons();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            NewDocument();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SaveDocument();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SaveAsDocument();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            PrintDocumentFile();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            PreviewPrint();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            CopyText();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            CutText();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            PasteText();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            UndoText();
        }
    }
}
