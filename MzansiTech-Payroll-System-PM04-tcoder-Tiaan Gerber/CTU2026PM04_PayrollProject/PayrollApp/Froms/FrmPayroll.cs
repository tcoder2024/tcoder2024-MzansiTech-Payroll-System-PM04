using System;
using System.Drawing;
using System.Windows.Forms;
using PayrollApp.Models;
using PayrollApp.Services;
using PayrollApp.Helpers;

namespace PayrollApp
{
    public partial class FrmPayroll : Form
    {
        private readonly CSVStorage _storage;
        private PayrollRecord? _currentRecord;
        private System.Windows.Forms.Timer? _successTimer;
        private System.Windows.Forms.Timer? _clockTimer;

        // Panels
        private Panel? _headerPanel;
        private Panel? _footerPanel;
        private Panel? _leftPanel;
        private Panel? _rightPanel;
        private Panel? _historyPanel;
        private Panel? _statusPanel;

        // Header controls
        private Label? _companyTitle;
        private Label? _companySubtitle;
        private Label? _clockLabel;
        private Label? _dateLabel;

        // Left Panel controls (Input)
        private TextBox? _txtName;
        private TextBox? _txtHours;
        private NumericUpDown? _numDependents;
        private Label? _lblHourlyRate;
        private Label? _lblHelpText;

        // Inline error labels (next to each field)
        private Label? _lblNameError;
        private Label? _lblHoursError;
        private Label? _lblDependentsError;

        // Buttons
        private Button? _btnCalculate;
        private Button? _btnReset;
        private Button? _btnSave;
        private Button? _btnViewHistory;
        private Button? _btnExit;

        // Right Panel controls (Results)
        private Label? _lblGrossValue;
        private Label? _lblUIFValue;
        private Label? _lblMembershipValue;
        private Label? _lblPAYEValue;
        private Label? _lblNetValue;

        // Status and error
        private Label? _lblError;
        private Label? _lblStatus;

        // History Grid
        private DataGridView? _dgvHistory;
        private Button? _btnCloseHistory;
        private Button? _btnClearHistory;

        public FrmPayroll()
        {
            _storage = new CSVStorage();
            _currentRecord = null;
            InitializeComponent();
            InitializeTimers();
            this.WindowState = FormWindowState.Maximized;
        }

        private void InitializeTimers()
        {
            _successTimer = new System.Windows.Forms.Timer();
            _successTimer.Interval = 3000;
            _successTimer.Tick += (s, e) => {
                if (_lblError != null) _lblError.Visible = false;
                _successTimer?.Stop();
            };

            _clockTimer = new System.Windows.Forms.Timer();
            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (s, e) => {
                if (_clockLabel != null) _clockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
                if (_dateLabel != null) _dateLabel.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            };
            _clockTimer.Start();
        }

        private void InitializeComponent()
        {
            // Form Settings
            this.Text = "Mzansi Tech Contractors Payroll Management System";
            this.BackColor = Color.FromArgb(236, 240, 245);
            this.MinimumSize = new Size(1200, 700);

            // ================================================================
            // HEADER PANEL
            // ================================================================
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.FromArgb(26, 43, 68),
                Padding = new Padding(30, 15, 30, 15)
            };

            _companyTitle = new Label
            {
                Text = "MZANSI TECH CONTRACTORS",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 20),
                AutoSize = true
            };

            _companySubtitle = new Label
            {
                Text = "South African Payroll Management System | SARS Compliant",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(180, 200, 240),
                Location = new Point(32, 60),
                AutoSize = true
            };

            _dateLabel = new Label
            {
                Text = DateTime.Now.ToString("dddd, dd MMMM yyyy"),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(180, 200, 240),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Size = new Size(250, 30),
                AutoSize = false
            };

            _clockLabel = new Label
            {
                Text = DateTime.Now.ToString("HH:mm:ss"),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 212, 180),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Size = new Size(250, 40),
                AutoSize = false
            };

            _headerPanel.Controls.Add(_companyTitle);
            _headerPanel.Controls.Add(_companySubtitle);
            _headerPanel.Controls.Add(_dateLabel);
            _headerPanel.Controls.Add(_clockLabel);

            // ================================================================
            // FOOTER PANEL
            // ================================================================
            _footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.FromArgb(26, 43, 68)
            };

            Label footerText = new Label
            {
                Text = "Hourly Rate: R950.00 | UIF: 1% | Membership Fee: 13% | PAYE: 25% (with dependent relief of 5.75% per dependent)",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(150, 175, 210),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            _footerPanel.Controls.Add(footerText);

            // ================================================================
            // MAIN CONTENT - Split into Left and Right panels
            // ================================================================
            Panel mainContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = Color.FromArgb(236, 240, 245)
            };

            // LEFT PANEL (55% width)
            _leftPanel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // RIGHT PANEL (40% width)
            _rightPanel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // ================================================================
            // LEFT PANEL - INPUT SECTION
            // ================================================================
            Label inputHeader = new Label
            {
                Text = "CONTRACTOR INFORMATION",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 43, 68),
                Location = new Point(30, 25),
                Size = new Size(400, 35)
            };

            Panel inputSeparator = new Panel
            {
                BackColor = Color.FromArgb(0, 160, 140),
                Location = new Point(30, 65),
                Size = new Size(100, 3)
            };

            // ================================================================
            // NAME FIELD with inline error
            // ================================================================
            Label lblName = new Label
            {
                Text = "Contractor Name",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 85, 100),
                Location = new Point(30, 90),
                Size = new Size(200, 25)
            };

            _txtName = new TextBox
            {
                Location = new Point(30, 120),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Enter full name"
            };

            // Inline error label for Name (appears right below the field)
            _lblNameError = new Label
            {
                Text = "❌ Contractor name is required",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(30, 158),
                Size = new Size(400, 18),
                Visible = false
            };

            // Clear error when user starts typing
            _txtName.TextChanged += (s, e) => {
                if (_lblNameError != null) _lblNameError.Visible = false;
                if (_txtName != null) _txtName.BackColor = Color.White;
            };

            // ================================================================
            // HOURS FIELD with inline error
            // ================================================================
            Label lblHours = new Label
            {
                Text = "Hours Worked",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 85, 100),
                Location = new Point(30, 190),
                Size = new Size(200, 25)
            };

            _txtHours = new TextBox
            {
                Location = new Point(30, 220),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Enter hours (0 - 744)"
            };

            // Inline error label for Hours
            _lblHoursError = new Label
            {
                Text = "❌ Hours must be between 0 and 744",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(30, 258),
                Size = new Size(400, 18),
                Visible = false
            };

            // Clear error when user starts typing
            _txtHours.TextChanged += (s, e) => {
                if (_lblHoursError != null) _lblHoursError.Visible = false;
                if (_txtHours != null) _txtHours.BackColor = Color.White;
            };

            // ================================================================
            // DEPENDENTS FIELD with inline error
            // ================================================================
            Label lblDependents = new Label
            {
                Text = "Number of Dependents",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 85, 100),
                Location = new Point(30, 285),
                Size = new Size(200, 25)
            };

            _numDependents = new NumericUpDown
            {
                Location = new Point(30, 315),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 12),
                Minimum = 0,
                Maximum = 10,
                BackColor = Color.White
            };

            Label dependentsHint = new Label
            {
                Text = "(0 to 10)",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(160, 322),
                Size = new Size(60, 25)
            };

            // Inline error label for Dependents
            _lblDependentsError = new Label
            {
                Text = "❌ Dependents must be between 0 and 10",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(30, 353),
                Size = new Size(400, 18),
                Visible = false
            };

            // Clear error when user changes value
            _numDependents.ValueChanged += (s, e) => {
                if (_lblDependentsError != null) _lblDependentsError.Visible = false;
                if (_numDependents != null) _numDependents.BackColor = Color.White;
            };

            // ================================================================
            // RATE INFO
            // ================================================================
            _lblHourlyRate = new Label
            {
                Text = "Current Hourly Rate: R950.00",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 160, 140),
                Location = new Point(30, 385),
                Size = new Size(250, 30)
            };

            _lblHelpText = new Label
            {
                Text = "Maximum 744 hours per month (31 days × 24 hours)",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(30, 415),
                Size = new Size(350, 25)
            };

            // ================================================================
            // BUTTONS
            // ================================================================
            _btnCalculate = new Button
            {
                Text = "CALCULATE PAYROLL",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 160, 140),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 50),
                Location = new Point(30, 460),
                Cursor = Cursors.Hand
            };
            _btnCalculate.Click += BtnCalculate_Click;

            _btnReset = new Button
            {
                Text = "RESET FORM",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(100, 110, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 50),
                Location = new Point(220, 460),
                Cursor = Cursors.Hand
            };
            _btnReset.Click += BtnReset_Click;

            _btnSave = new Button
            {
                Text = "SAVE RECORD",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 50),
                Location = new Point(30, 530),
                Enabled = false,
                Cursor = Cursors.Hand
            };
            _btnSave.Click += BtnSave_Click;

            _btnViewHistory = new Button
            {
                Text = "VIEW HISTORY",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 50),
                Location = new Point(190, 530),
                Cursor = Cursors.Hand
            };
            _btnViewHistory.Click += BtnToggleHistory_Click;

            _btnExit = new Button
            {
                Text = "EXIT SYSTEM",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 60, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 50),
                Location = new Point(350, 530),
                Cursor = Cursors.Hand
            };
            _btnExit.Click += (s, e) => Application.Exit();

            // Add all controls to left panel
            _leftPanel.Controls.Add(inputHeader);
            _leftPanel.Controls.Add(inputSeparator);
            _leftPanel.Controls.Add(lblName);
            _leftPanel.Controls.Add(_txtName);
            _leftPanel.Controls.Add(_lblNameError);
            _leftPanel.Controls.Add(lblHours);
            _leftPanel.Controls.Add(_txtHours);
            _leftPanel.Controls.Add(_lblHoursError);
            _leftPanel.Controls.Add(lblDependents);
            _leftPanel.Controls.Add(_numDependents);
            _leftPanel.Controls.Add(dependentsHint);
            _leftPanel.Controls.Add(_lblDependentsError);
            _leftPanel.Controls.Add(_lblHourlyRate);
            _leftPanel.Controls.Add(_lblHelpText);
            _leftPanel.Controls.Add(_btnCalculate);
            _leftPanel.Controls.Add(_btnReset);
            _leftPanel.Controls.Add(_btnSave);
            _leftPanel.Controls.Add(_btnViewHistory);
            _leftPanel.Controls.Add(_btnExit);

            // ================================================================
            // RIGHT PANEL - RESULTS SECTION (unchanged)
            // ================================================================
            Label resultHeader = new Label
            {
                Text = "PAYROLL SUMMARY",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 43, 68),
                Location = new Point(30, 25),
                Size = new Size(300, 35)
            };

            Panel resultSeparator = new Panel
            {
                BackColor = Color.FromArgb(0, 160, 140),
                Location = new Point(30, 65),
                Size = new Size(100, 3)
            };

            // Gross Pay
            Label lblGrossTitle = new Label
            {
                Text = "GROSS PAY",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 85, 100),
                Location = new Point(30, 100),
                Size = new Size(200, 30)
            };

            _lblGrossValue = new Label
            {
                Text = "R 0.00",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 160, 140),
                Location = new Point(250, 95),
                Size = new Size(200, 40),
                TextAlign = ContentAlignment.MiddleRight
            };

            // UIF
            Label lblUIFTitle = new Label
            {
                Text = "UIF (1%)",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(80, 85, 100),
                Location = new Point(30, 150),
                Size = new Size(200, 30)
            };

            _lblUIFValue = new Label
            {
                Text = "R 0.00",
                Font = new Font("Segoe UI", 13),
                ForeColor = Color.FromArgb(80, 85, 100),
                Location = new Point(250, 150),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Membership
            Label lblMembershipTitle = new Label
            {
                Text = "Membership Fee (13%)",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(80, 85, 100),
                Location = new Point(30, 195),
                Size = new Size(200, 30)
            };

            _lblMembershipValue = new Label
            {
                Text = "R 0.00",
                Font = new Font("Segoe UI", 13),
                ForeColor = Color.FromArgb(80, 85, 100),
                Location = new Point(250, 195),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleRight
            };

            // PAYE
            Label lblPAYETitle = new Label
            {
                Text = "PAYE (25%)",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(80, 85, 100),
                Location = new Point(30, 240),
                Size = new Size(200, 30)
            };

            _lblPAYEValue = new Label
            {
                Text = "R 0.00",
                Font = new Font("Segoe UI", 13),
                ForeColor = Color.FromArgb(80, 85, 100),
                Location = new Point(250, 240),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Separator line
            Panel sepLine = new Panel
            {
                BackColor = Color.FromArgb(220, 225, 230),
                Location = new Point(30, 290),
                Size = new Size(420, 2)
            };

            // Net Pay
            Label lblNetTitle = new Label
            {
                Text = "NET PAY",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 43, 68),
                Location = new Point(30, 310),
                Size = new Size(200, 45)
            };

            _lblNetValue = new Label
            {
                Text = "R 0.00",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 160, 140),
                Location = new Point(200, 305),
                Size = new Size(250, 55),
                TextAlign = ContentAlignment.MiddleRight
            };

            _rightPanel.Controls.Add(resultHeader);
            _rightPanel.Controls.Add(resultSeparator);
            _rightPanel.Controls.Add(lblGrossTitle);
            _rightPanel.Controls.Add(_lblGrossValue);
            _rightPanel.Controls.Add(lblUIFTitle);
            _rightPanel.Controls.Add(_lblUIFValue);
            _rightPanel.Controls.Add(lblMembershipTitle);
            _rightPanel.Controls.Add(_lblMembershipValue);
            _rightPanel.Controls.Add(lblPAYETitle);
            _rightPanel.Controls.Add(_lblPAYEValue);
            _rightPanel.Controls.Add(sepLine);
            _rightPanel.Controls.Add(lblNetTitle);
            _rightPanel.Controls.Add(_lblNetValue);

            // ================================================================
            // STATUS AND ERROR PANEL
            // ================================================================
            _statusPanel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            _lblError = new Label
            {
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(15, 10),
                Size = new Size(700, 25),
                Visible = false
            };

            _lblStatus = new Label
            {
                Text = "System ready. Enter contractor details and click CALCULATE PAYROLL.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 110, 130),
                Location = new Point(15, 35),
                Size = new Size(700, 20)
            };

            _statusPanel.Controls.Add(_lblError);
            _statusPanel.Controls.Add(_lblStatus);

            // ================================================================
            // HISTORY PANEL (Bottom, initially hidden)
            // ================================================================
            _historyPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 300,
                BackColor = Color.White,
                Visible = false
            };

            Panel historyHeader = new Panel
            {
                BackColor = Color.FromArgb(26, 43, 68),
                Dock = DockStyle.Top,
                Height = 45
            };

            Label historyTitle = new Label
            {
                Text = "PAYROLL HISTORY",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 10),
                AutoSize = true
            };

            _btnClearHistory = new Button
            {
                Text = "CLEAR ALL",
                BackColor = Color.FromArgb(220, 60, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(90, 30),
                Cursor = Cursors.Hand
            };
            _btnClearHistory.Click += BtnClearHistory_Click;

            _btnCloseHistory = new Button
            {
                Text = "CLOSE",
                BackColor = Color.FromArgb(100, 110, 130),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 30),
                Cursor = Cursors.Hand
            };
            _btnCloseHistory.Click += (s, e) => {
                _historyPanel.Visible = false;
                if (_btnViewHistory != null) _btnViewHistory.Text = "VIEW HISTORY";
            };

            historyHeader.Controls.Add(historyTitle);
            historyHeader.Controls.Add(_btnClearHistory);
            historyHeader.Controls.Add(_btnCloseHistory);

            _dgvHistory = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(248, 249, 250) }
            };

            _dgvHistory.Columns.Add("Name", "Contractor Name");
            _dgvHistory.Columns.Add("Hours", "Hours");
            _dgvHistory.Columns.Add("GrossPay", "Gross Pay");
            _dgvHistory.Columns.Add("NetPay", "Net Pay");
            _dgvHistory.Columns.Add("Date", "Date Calculated");

            _historyPanel.Controls.Add(_dgvHistory);
            _historyPanel.Controls.Add(historyHeader);

            // Add all to main content
            mainContent.Controls.Add(_leftPanel);
            mainContent.Controls.Add(_rightPanel);
            mainContent.Controls.Add(_statusPanel);

            // Add to form
            this.Controls.Add(_headerPanel);
            this.Controls.Add(mainContent);
            this.Controls.Add(_footerPanel);
            this.Controls.Add(_historyPanel);

            // Handle resize events
            this.Resize += (s, e) => LayoutPanels(mainContent);
            this.Load += (s, e) => LayoutPanels(mainContent);
        }

        private void LayoutPanels(Panel mainContent)
        {
            if (_leftPanel == null || _rightPanel == null || _statusPanel == null || mainContent == null) return;

            int margin = 30;
            int statusHeight = 80;
            int contentHeight = mainContent.ClientSize.Height - statusHeight - margin;
            int contentWidth = mainContent.ClientSize.Width - (margin * 2);

            int leftWidth = contentWidth * 55 / 100;
            int rightWidth = contentWidth - leftWidth - margin;

            _leftPanel.SetBounds(margin, margin, leftWidth, contentHeight);
            _rightPanel.SetBounds(margin + leftWidth + margin, margin, rightWidth, contentHeight);
            _statusPanel.SetBounds(margin, margin + contentHeight + margin, contentWidth, statusHeight);

            if (_clockLabel != null && _dateLabel != null)
            {
                _clockLabel.Left = this.ClientSize.Width - 280;
                _dateLabel.Left = this.ClientSize.Width - 280;
            }

            if (_btnClearHistory != null && _btnCloseHistory != null)
            {
                _btnClearHistory.Left = this.ClientSize.Width - 130;
                _btnCloseHistory.Left = this.ClientSize.Width - 230;
            }
        }

        private void ClearAllInlineErrors()
        {
            if (_lblNameError != null) _lblNameError.Visible = false;
            if (_lblHoursError != null) _lblHoursError.Visible = false;
            if (_lblDependentsError != null) _lblDependentsError.Visible = false;

            if (_txtName != null) _txtName.BackColor = Color.White;
            if (_txtHours != null) _txtHours.BackColor = Color.White;
            if (_numDependents != null) _numDependents.BackColor = Color.White;
        }

        private void BtnCalculate_Click(object? sender, EventArgs e)
        {
            try
            {
                // Clear all previous inline errors
                ClearAllInlineErrors();
                HideError();

                bool hasError = false;

                // Validate name with INLINE error
                string name = _txtName?.Text.Trim() ?? string.Empty;
                if (!ValidationHelper.IsValidName(name))
                {
                    if (_lblNameError != null)
                    {
                        _lblNameError.Text = "❌ Contractor name is required";
                        _lblNameError.Visible = true;
                    }
                    if (_txtName != null) _txtName.BackColor = Color.FromArgb(255, 240, 240);
                    _txtName?.Focus();
                    hasError = true;
                }

                // Validate hours with INLINE error
                double hours = 0;
                bool hoursValid = true;
                if (_txtHours == null || !ValidationHelper.TryParseHours(_txtHours.Text, out hours))
                {
                    if (_lblHoursError != null)
                    {
                        _lblHoursError.Text = "❌ Please enter a valid number for hours";
                        _lblHoursError.Visible = true;
                    }
                    if (_txtHours != null) _txtHours.BackColor = Color.FromArgb(255, 240, 240);
                    hoursValid = false;
                    hasError = true;
                }
                else if (!ValidationHelper.IsValidHours(hours))
                {
                    if (_lblHoursError != null)
                    {
                        _lblHoursError.Text = "❌ Hours must be between 0 and 744";
                        _lblHoursError.Visible = true;
                    }
                    if (_txtHours != null) _txtHours.BackColor = Color.FromArgb(255, 240, 240);
                    hoursValid = false;
                    hasError = true;
                }

                // Validate dependents with INLINE error
                int dependents = _numDependents != null ? (int)_numDependents.Value : 0;
                if (!ValidationHelper.IsValidDependents(dependents))
                {
                    if (_lblDependentsError != null)
                    {
                        _lblDependentsError.Text = "❌ Dependents must be between 0 and 10";
                        _lblDependentsError.Visible = true;
                    }
                    if (_numDependents != null) _numDependents.BackColor = Color.FromArgb(255, 240, 240);
                    hasError = true;
                }

                if (hasError)
                {
                    if (_lblStatus != null) _lblStatus.Text = "Please correct the errors highlighted in red.";
                    return;
                }

                // Create and calculate record
                _currentRecord = new PayrollRecord
                {
                    ContractorName = name,
                    HoursWorked = hours,
                    Dependents = dependents,
                    CalculationDate = DateTime.Now
                };

                _currentRecord.GrossPay = PayrollCalculator.CalculateGrossPay(hours);
                _currentRecord.UIFDeduction = PayrollCalculator.CalculateUIF(_currentRecord.GrossPay);
                _currentRecord.MembershipFee = PayrollCalculator.CalculateMembershipFee(_currentRecord.GrossPay);
                _currentRecord.PAYE = PayrollCalculator.CalculatePAYE(_currentRecord.GrossPay, dependents);
                _currentRecord.NetPay = PayrollCalculator.CalculateNetPay(
                    _currentRecord.GrossPay,
                    _currentRecord.UIFDeduction,
                    _currentRecord.MembershipFee,
                    _currentRecord.PAYE);

                // Update UI
                if (_lblGrossValue != null) _lblGrossValue.Text = $"R {_currentRecord.GrossPay:N2}";
                if (_lblUIFValue != null) _lblUIFValue.Text = $"R {_currentRecord.UIFDeduction:N2}";
                if (_lblMembershipValue != null) _lblMembershipValue.Text = $"R {_currentRecord.MembershipFee:N2}";
                if (_lblPAYEValue != null) _lblPAYEValue.Text = $"R {_currentRecord.PAYE:N2}";

                if (_lblNetValue != null)
                {
                    _lblNetValue.Text = $"R {_currentRecord.NetPay:N2}";

                    if (_currentRecord.NetPay > 50000)
                        _lblNetValue.ForeColor = Color.FromArgb(46, 204, 113);
                    else if (_currentRecord.NetPay > 25000)
                        _lblNetValue.ForeColor = Color.FromArgb(0, 160, 140);
                    else if (_currentRecord.NetPay > 10000)
                        _lblNetValue.ForeColor = Color.FromArgb(241, 196, 15);
                    else
                        _lblNetValue.ForeColor = Color.FromArgb(231, 76, 60);
                }

                if (_btnSave != null) _btnSave.Enabled = true;
                if (_lblStatus != null) _lblStatus.Text = "Calculation completed successfully.";
                ShowSuccess("SUCCESS: Calculation completed successfully!");
            }
            catch (Exception ex)
            {
                ShowError($"ERROR: Calculation failed - {ex.Message}");
                if (_lblStatus != null) _lblStatus.Text = "Calculation failed.";
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentRecord == null)
                {
                    ShowError("ERROR: Please calculate payroll before saving.");
                    return;
                }

                _storage.SaveRecord(_currentRecord);
                ShowSuccess("SUCCESS: Payroll record saved successfully!");
                if (_btnSave != null) _btnSave.Enabled = false;
                if (_lblStatus != null) _lblStatus.Text = "Record saved to storage.";
                RefreshHistoryGrid();
            }
            catch (Exception ex)
            {
                ShowError($"ERROR: Save failed - {ex.Message}");
                if (_lblStatus != null) _lblStatus.Text = "Save failed.";
            }
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            // Clear input fields
            if (_txtName != null) _txtName.Clear();
            if (_txtHours != null) _txtHours.Clear();
            if (_numDependents != null) _numDependents.Value = 0;

            // Clear all inline errors
            ClearAllInlineErrors();

            // Reset result displays
            if (_lblGrossValue != null) _lblGrossValue.Text = "R 0.00";
            if (_lblUIFValue != null) _lblUIFValue.Text = "R 0.00";
            if (_lblMembershipValue != null) _lblMembershipValue.Text = "R 0.00";
            if (_lblPAYEValue != null) _lblPAYEValue.Text = "R 0.00";
            if (_lblNetValue != null)
            {
                _lblNetValue.Text = "R 0.00";
                _lblNetValue.ForeColor = Color.FromArgb(0, 160, 140);
            }

            // Reset state
            HideError();
            if (_btnSave != null) _btnSave.Enabled = false;
            _currentRecord = null;
            if (_lblStatus != null) _lblStatus.Text = "Form reset. Ready for new entry.";
            if (_txtName != null) _txtName.Focus();

            ShowSuccess("SUCCESS: Form has been reset.");
        }

        private void BtnToggleHistory_Click(object? sender, EventArgs e)
        {
            if (_historyPanel != null)
            {
                bool isVisible = _historyPanel.Visible;
                _historyPanel.Visible = !isVisible;
                if (_btnViewHistory != null)
                {
                    _btnViewHistory.Text = isVisible ? "VIEW HISTORY" : "HIDE HISTORY";
                }
                if (!isVisible)
                {
                    RefreshHistoryGrid();
                }
            }
        }

        private void BtnClearHistory_Click(object? sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "WARNING: This will permanently delete ALL payroll records.\n\nAre you sure you want to continue?",
                "Confirm Clear All Records",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _storage.ClearHistory();
                RefreshHistoryGrid();
                ShowSuccess("SUCCESS: All history records have been cleared.");
                if (_lblStatus != null) _lblStatus.Text = "History cleared. All records deleted.";
            }
        }

        private void RefreshHistoryGrid()
        {
            if (_dgvHistory == null || _storage == null) return;

            _dgvHistory.Rows.Clear();
            var records = _storage.GetAllRecords();

            if (records.Count == 0)
            {
                _dgvHistory.Rows.Add("No records found", "", "", "", "");
                if (_lblStatus != null) _lblStatus.Text = "History is empty. No records to display.";
            }
            else
            {
                foreach (var record in records)
                {
                    _dgvHistory.Rows.Add(
                        record.ContractorName,
                        record.HoursWorked,
                        $"R {record.GrossPay:N2}",
                        $"R {record.NetPay:N2}",
                        record.CalculationDate.ToString("dd MMM yyyy HH:mm")
                    );
                }
                if (_lblStatus != null) _lblStatus.Text = $"History loaded: {records.Count} record(s) found.";
            }
        }

        private void ShowError(string message)
        {
            if (_lblError != null)
            {
                _lblError.Text = message;
                _lblError.ForeColor = Color.FromArgb(231, 76, 60);
                _lblError.Visible = true;
                _successTimer?.Stop();
            }
        }

        private void ShowSuccess(string message)
        {
            if (_lblError != null)
            {
                _lblError.Text = message;
                _lblError.ForeColor = Color.FromArgb(46, 204, 113);
                _lblError.Visible = true;
                _successTimer?.Start();
            }
        }

        private void HideError()
        {
            if (_lblError != null) _lblError.Visible = false;
            _successTimer?.Stop();
        }
    }
}