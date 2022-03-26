#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeRegistration.Context;
using EmployeeRegistration.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeeRegistration.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeContext _context;

        public EmployeesController(EmployeeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index( DateTime? FromDate = null , DateTime? ToDate = null, string? Gender = null)
        {
            var List=new List<Employee>();
            var listquery=_context.Employees.AsQueryable();
            if (FromDate != null && ToDate != null)
            {
                listquery=listquery.Where(x => x.DateOfBirth <= ToDate && x.DateOfBirth >= FromDate);
            }
            if(Gender != null)
            {
                listquery = listquery.Where(x => x.Gender == Gender);
            }
            List = listquery.ToList();
            return View(List);
        }


        public IActionResult Create()
        {
            var emp = new Employee();

            return View(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {

            for (int i = 0; i < employee.Educations.Count; i++)
            {
                if (employee.Educations[i].IsDeleted)
                {

                    ModelState.Remove($"Educations[{i}].yearOfPassing");
                    ModelState.Remove($"Educations[{i}].DegreeName");
                }

            }
            if (ModelState.IsValid)
            {
                employee.DateOfBirth=employee.DateOfBirth.ToUniversalTime();
                _context.Add(employee);

                await _context.SaveChangesAsync();
                foreach (var item in employee.Educations.Where(x=>x.IsDeleted==false))
                {
                    item.EmployeeId = employee.EmployeeId;
                    if (item.Id == 0)
                    {
                        _context.Add(item);
                    }
                    else
                    {
                        _context.Update(item);
                    }
                }
                await _context.SaveChangesAsync();
                ViewBag.MessageError = "Save Successfully";
                return RedirectToAction("Index");
            }
            return View(employee);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEduction(Employee employee)
        {
            for (int i = 0; i < employee.Educations.Count; i++)
            {
                if (employee.Educations[i].IsDeleted)
                {

                    ModelState.Remove($"Educations[{i}].yearOfPassing");
                    ModelState.Remove($"Educations[{i}].DegreeName");
                }

            }

            if (ModelState.IsValid)
            {
                employee.Educations.Add(new EmployeeEducation());
                return View("Create", employee);
            }
            return View("Create", employee);
        }


        public async Task<IActionResult> Edit(int? id)
        {

            var employee = await _context.Employees.FindAsync(id);
            employee.Educations= new List<EmployeeEducation>();
            employee.Educations.AddRange(_context.EmployeesEducations.Where(x=>x.EmployeeId==id).AsNoTracking().ToList());
            return View("Create", employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {

            if (ModelState.IsValid)
            {
                employee.DateOfBirth = employee.DateOfBirth.ToUniversalTime();
                foreach (var item in employee.Educations)
                {
                    item.EmployeeId = id;
                    if (item.Id == 0 && item.IsDeleted==false)
                    {
                        _context.Add(item);
                    }
                    else if (item.IsDeleted == false)
                    {
                        _context.Update(item);
                    }else if( item.IsDeleted == true && item.Id>0)
                    {
                        _context.Remove(item);
                    }
                }
                _context.Update(employee);
                await _context.SaveChangesAsync();
                ViewBag.MessageError = "Save Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View("Create", employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
           
            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            if (employee == null)
            {
                ViewBag.MessageError = "Not Found";
            }

            return RedirectToAction("Index");
        }

    }
}
