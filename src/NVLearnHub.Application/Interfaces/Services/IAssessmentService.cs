using NVLearnHub.Application.DTOs.Assessment;

namespace NVLearnHub.Application.Interfaces.Services
{
    public interface IAssessmentService
    {
        Task<ApiResponse<IEnumerable<AssessmentDto>>> GetByCourseAsync(int courseId);
        Task<ApiResponse<IEnumerable<AssessmentDto>>> GetForStudentAsync(int courseId);
        Task<ApiResponse<AssessmentDto>> CreateAssessmentAsync(CreateAssessmentDto dto);
        Task<ApiResponse<AssessmentDto>> AddQuestionsAsync(int assessmentId, List<CreateQuestionDto> questions);
        Task<ApiResponse<bool>> DeleteAssessmentAsync(int assessmentId);
        Task<ApiResponse<StartAttemptResponseDto>> StartAttemptAsync(int assessmentId, int userId);
        Task<ApiResponse<AssessmentResultDto>> SubmitAsync(SubmitAssessmentDto dto, int userId);
        Task<ApiResponse<AssessmentResultDto>> GetAttemptResultAsync(int attemptId, int userId);
        Task<ApiResponse<AssessmentStatusDto>> GetStatusAsync(int courseId, int userId);
        Task<ApiResponse<IEnumerable<AttemptHistoryDto>>> GetAttemptHistoryAsync(int courseId, int userId);
    }
}