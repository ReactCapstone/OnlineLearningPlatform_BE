using NVLearnHub.Application.DTOs.Assessment;

namespace NVLearnHub.Application.Interfaces.Services
{
    public interface IAssessmentService
    {
        Task<ApiResponse<AssessmentDto>> GetByCourseAsync(int courseId);
        Task<ApiResponse<StartAttemptResponseDto>> StartAttemptAsync(int assessmentId, int userId);
        Task<ApiResponse<AssessmentResultDto>> SubmitAsync(SubmitAssessmentDto dto, int userId);
        Task<ApiResponse<AssessmentResultDto>> GetAttemptResultAsync(int attemptId, int userId);
    }
}