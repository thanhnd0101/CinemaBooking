﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace LotteCinema
{
    public partial class fManager : Form
    {
        int employeeID;
        int index = -1;
        public fManager()
        {
            InitializeComponent();
            employeeID = fLogin.employeeID;
        }

        private void loadTabFilm(SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand("sp_LietKePhim", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv_film.DataSource = dt;
            }
        }

        private void loadTabShowtimes(SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand("sp_LietKeSuatChieu", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@soluong", SqlDbType.Int);
                cmd.Parameters["@soluong"].Direction = ParameterDirection.Output;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv_showtime.DataSource = dt;
            }
        }

        private string Date(DateTime dt)
        {
            return dt.Day.ToString() + "/" + dt.Month.ToString() + "/" + dt.Year.ToString();
        }

        private void loadTabStatistic(SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand("sp_LietKePhim", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cb_film.DataSource = dt;
                cb_film.ValueMember = "idphim";
                cb_film.DisplayMember = "tuaphim";
            }

            dtpk_dateFrom.Value = DateTime.Today;
            tb_dateFrom.Text = Date(dtpk_dateFrom.Value);
            dtpk_dateTo.Value = DateTime.Today.AddDays(1);
            tb_dateTo.Text = Date(dtpk_dateTo.Value);
        }

        private int getCinemaID()
        {
            int cinemaID = 0;
            using (SqlConnection conn = new SqlConnection(SQLConnection.connectionString()))
            using (SqlCommand cmd = new SqlCommand("sp_LamViecORap", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@nhanvien", SqlDbType.Int);
                cmd.Parameters.Add("@rap", SqlDbType.Int);
                cmd.Parameters["@nhanvien"].Value = employeeID;
                cmd.Parameters["@rap"].Direction = ParameterDirection.Output;
                conn.Open();
                cmd.ExecuteScalar();
                cinemaID = (int)cmd.Parameters["@rap"].Value;
                conn.Close();
            }
            return cinemaID;
        }

        private void fManager_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(SQLConnection.connectionString()))
            {
                conn.Open();
                loadTabFilm(conn);
                loadTabShowtimes(conn);
                loadTabStatistic(conn);
                conn.Close();
            }
        }

        private void btn_statistic_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(SQLConnection.connectionString()))
            using (SqlCommand cmd = new SqlCommand("sp_ThongKe", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                //MessageBox.Show(cb_film.SelectedValue.ToString(), "id film");
                cmd.Parameters.Add("@rap", SqlDbType.Int);
                cmd.Parameters.Add("@phim", SqlDbType.Int);
                cmd.Parameters.Add("@ngayBD", SqlDbType.Date);
                cmd.Parameters.Add("@ngayKT", SqlDbType.Date);
                cmd.Parameters["@rap"].Value = getCinemaID();
                cmd.Parameters["@phim"].Value = int.Parse(cb_film.SelectedValue.ToString());
                cmd.Parameters["@ngayBD"].Value = dtpk_dateFrom.Value.Date;
                cmd.Parameters["@ngayKT"].Value = dtpk_dateTo.Value.Date;
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv_statistic.DataSource = dt;
                conn.Close();
            }
        }

        private void dtpk_dateFrom_ValueChanged(object sender, EventArgs e)
        {
            tb_dateFrom.Text = Date(dtpk_dateFrom.Value);
        }

        private void dtpk_dateTo_ValueChanged(object sender, EventArgs e)
        {
            tb_dateTo.Text = Date(dtpk_dateTo.Value);
        }

        private void dgv_film_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           index = e.RowIndex;
        }

        private void btn_insertFilm_Click(object sender, EventArgs e)
        {
            AddFilm addfilm = new AddFilm();
            addfilm.Show();
            fManager_Load(sender,e);
        }

        private void tb_dateFrom_TextChanged(object sender, EventArgs e)
        {

        }

        private void lb_startday_Click(object sender, EventArgs e)
        {

        }

        private void btn_updateFilm_Click(object sender, EventArgs e)
        {
            if(index != -1)
            {
                DataGridViewRow selectedRow = dgv_film.Rows[index];
                int idPhim =int.Parse(selectedRow.Cells[0].Value.ToString());
                String tuaphim = selectedRow.Cells[1].Value.ToString();
                String theloai = selectedRow.Cells[2].Value.ToString();
                String daodien = selectedRow.Cells[3].Value.ToString();
                DateTime ngaycongchieu = DateTime.Parse(selectedRow.Cells[4].Value.ToString().Replace("/", "-"));
                int thoiluong = int.Parse(selectedRow.Cells[5].Value.ToString());
                String nuocsx = selectedRow.Cells[6].Value.ToString();
                int namsx = int.Parse(selectedRow.Cells[7].Value.ToString());
                String tinhtrang = selectedRow.Cells[8].Value.ToString();

                UpdateFilm updateFilm = new UpdateFilm(idPhim, tuaphim, theloai, daodien, ngaycongchieu, thoiluong, nuocsx,
                    namsx, tinhtrang);
                updateFilm.Show();

            }
            else
            {
                MessageBox.Show("Vui long chon Phim");
            }
        }
    }
}
