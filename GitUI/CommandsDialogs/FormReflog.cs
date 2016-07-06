using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GitCommands;
using GitUI.HelperDialogs;
using ResourceManager;

namespace GitUI.CommandsDialogs
{
    public partial class FormReflog : GitModuleForm
    {
        //private readonly TranslationString _continueRebaseText = new TranslationString("Continue rebase");
        //private readonly TranslationString _solveConflictsText = new TranslationString("Solve conflicts");

        //private readonly TranslationString _solveConflictsText2 = new TranslationString(">Solve conflicts<");
        //private readonly TranslationString _continueRebaseText2 = new TranslationString(">Continue rebase<");

        //private readonly TranslationString _noBranchSelectedText = new TranslationString("Please select a branch");

        //private readonly TranslationString _branchUpToDateText =
        //    new TranslationString("Current branch a is up to date." + Environment.NewLine + "Nothing to rebase.");
        //private readonly TranslationString _branchUpToDateCaption = new TranslationString("Rebase");

        //private readonly TranslationString _hoverShowImageLabelText = new TranslationString("Hover to see scenario when fast forward is possible.");

        private readonly string _currentBranch;
        readonly Regex _regexReflog = new Regex("^([^ ]+) ([^:]+): ([^:]+): (.+)$", RegexOptions.Compiled);

        public FormReflog(GitUICommands uiCommands, string currentBranch)
            : base(uiCommands)
        {
            InitializeComponent();
            Translate();
            _currentBranch = currentBranch;

            var reflogOutput = uiCommands.GitModule.RunGitCmd("reflog");
            var reflog = ConvertReflogOutput(reflogOutput);
            gridReflog.DataSource = reflog;
        }

        public bool ShouldRefresh { get; set; }

        private List<RefLine> ConvertReflogOutput(string reflogOutput)
        {
            var refLog = new List<RefLine>();
            foreach (var line in reflogOutput.Split('\n'))
            {
                var match = _regexReflog.Match(line);
                refLog.Add(new RefLine
                {
                    Sha = match.Groups[1].Value,
                    Ref = match.Groups[2].Value,
                    Action = match.Groups[3].Value,
                    Message = match.Groups[4].Value
                });
            }
            return refLog;
        }

        private void createABranchOnThisCommitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridReflog.SelectedCells.Count == 0 && gridReflog.SelectedRows.Count == 0)
            {
                return;
            }

            UICommands.GitCommand("branch save " + GetShaOfRefLine());
            ShouldRefresh = true;
        }

        private string GetShaOfRefLine()
        {
            var row = GetSelectedRow();
            var refLine = (RefLine) row.DataBoundItem;
            return refLine.Sha;
        }

        private DataGridViewRow GetSelectedRow()
        {
            if (gridReflog.SelectedRows.Count > 0)
            {
                return gridReflog.SelectedRows[0];
            }
            return gridReflog.Rows[gridReflog.SelectedCells[0].RowIndex];
        }

        private void resetCurrentBranchOnThisCommitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UICommands.GitCommand("reset --hard " + GetShaOfRefLine());
            ShouldRefresh = true;
        }
    }

    internal class RefLine
    {
        public string Sha { get; set; }
        public string Ref { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
    }
}
