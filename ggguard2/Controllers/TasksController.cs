using ggguard2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ggguard2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private static readonly List<TaskItem> tasks = new()
        {
            new TaskItem
            {
                Id = 1,
                Title = "Изучить ASP.NET Core",
                Description = "Разобраться с Web API",
                IsCompleted = false
            },
            new TaskItem
            {
                Id = 2,
                Title = "Настроить JWT",
                Description = "Реализовать аутентификацию",
                IsCompleted = true
            },
            new TaskItem
            {
                Id = 3,
                Title = "Проверить API",
                Description = "Протестировать защищённые методы",
                IsCompleted = false
            }
        };

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound(new
                {
                    message = "Задача не найдена"
                });
            }

            return Ok(task);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] TaskItem newTask)
        {
            if (string.IsNullOrWhiteSpace(newTask.Title))
            {
                return BadRequest(new
                {
                    message = "Название задачи обязательно"
                });
            }

            newTask.Id = tasks.Count == 0
                ? 1
                : tasks.Max(t => t.Id) + 1;

            tasks.Add(newTask);

            var username = HttpContext.User
                .FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new
            {
                message = "Задача успешно создана",
                createdBy = username,
                task = newTask
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound(new
                {
                    message = "Задача не найдена"
                });
            }

            tasks.Remove(task);

            var username = HttpContext.User
                .FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new
            {
                message = "Задача успешно удалена",
                deletedTaskId = id,
                deletedBy = username
            });
        }
    }
}