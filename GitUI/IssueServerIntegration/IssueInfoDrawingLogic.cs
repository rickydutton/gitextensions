using System;
using System.Drawing;
using System.Windows.Forms;
using GitCommands;
using GitUI.Properties;
using GitUIPluginInterfaces.IssueTrackerIntegration;

namespace GitUI.IssueTrackerIntegrationIntegration
{
    static internal class IssueInfoDrawingLogic
    {
        /*
        public static void IssueStatusImageColumnCellPainting(DataGridViewCellPaintingEventArgs e, GitRevision revision, Brush foreBrush, Font rowFont)
        {
            if (revision.IssueStatus != null)
            {
                Image issueStatusImage = null;

                switch (revision.IssueStatus)
                {
                    // Todo:
                     
                }

                if (issueStatusImage != null)
                {
                    e.Graphics.DrawImage(issueSatusImage, new Rectangle(e.CellBounds.Left, e.CellBounds.Top + 4, 16, 16));
                }
            }
        }
        */

        public static void IssueStatusMessageCellPainting(DataGridViewCellPaintingEventArgs e, GitRevision revision, Brush foreBrush, Font rowFont)
        {
            if (revision.IssueStatus != null)
            {
                Brush IssueStatusForebrush = foreBrush;

                switch (revision.IssueStatus)
                {
                    // Todo: Fix string literals
                    case "Passed QA":
                        IssueStatusForebrush = Brushes.DarkGreen;
                        break;
                    case "To Do":
                        IssueStatusForebrush = Brushes.DarkRed;
                        break;
                    case "Ready For QA":
                    case "In UAT":
                        IssueStatusForebrush = Brushes.Blue;
                        break;
                    case "Blocked":
                        IssueStatusForebrush = Brushes.OrangeRed;
                        break;
                    case "Closed":
                        IssueStatusForebrush = Brushes.Gray;
                        break;
                }

                var text = (string)e.FormattedValue;
                e.Graphics.DrawString(text, rowFont, IssueStatusForebrush, new PointF(e.CellBounds.Left, e.CellBounds.Top + 4));
            }
        }

        public static void IssueStatusImageColumnCellFormatting(DataGridViewCellFormattingEventArgs e, DataGridView grid, GitRevision revision)
        {
            e.FormattingApplied = false;
            var cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            cell.ToolTipText = GetIssueStatusMessageText(revision);
        }

        public static void IssueStatusMessageCellFormatting(DataGridViewCellFormattingEventArgs e, GitRevision revision)
        {
            e.Value = GetIssueStatusMessageText(revision);
        }

        private static string GetIssueStatusMessageText(GitRevision revision)
        {
            if (revision.IssueStatus == null || string.IsNullOrEmpty(revision.IssueStatus))
                return string.Empty;
            return revision.IssueStatus;
        }
    }
}