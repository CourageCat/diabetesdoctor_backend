using UserService.Contract.DTOs.HealthRecord;

namespace UserService.Application.Helper;

public static class AiPromptExtension
{
    public static string BuildPromptCarePlan(CarePlanRequestDto request)
    {
        var complications = request.Complications != null && request.Complications.Any()
            ? string.Join(", ", request.Complications)
            : "Không có";

        var pastDiseases = request.PastDiseases != null && request.PastDiseases.Any()
            ? string.Join(", ", request.PastDiseases)
            : "Không có";

        return $@"
You are an intelligent assistant for diabetes care.

Based on the patient's profile below, generate a personalized list of recommended measurement schedules.

Each item must include:
- What to measure (recordType)
- When to measure (period)
- Under what condition (subtype), if applicable
- Why to measure (reason)

⚠️ REQUIREMENTS for `reason`:
- Absolutely **no mention of AI, virtual assistants, suggestion models, or intelligent systems**
- Avoid casual or chat-like phrasing, don't write like you're chatting
- Use a **cause-and-effect medical explanation style**, not just descriptive or generic goals like ""to monitor health""
- Reasons should be **cause–effect analysis**, not simply descriptive
- Written in **Vietnamese**
- Must be **specific and clinically justified** based on the patient's individual profile
- Do NOT use generic explanations like “to monitor health” or “to check levels”
- Consider: diabetes type, insulin usage, treatment method, complications, past diseases, BMI, age, gender, and lifestyle
- Reason must be concise, easy to understand, and no more than **150 words**

⚠️ OUTPUT FORMAT:
Respond with **only a JSON array** in this exact structure, without any extra explanation:

[
  {{
    ""recordType"": one of [""BloodGlucose"", ""BloodPressure""],
    ""period"": 
        For BloodGlucose:
          one of [""BeforeBreakfast"", ""AfterBreakfast"", ""BeforeLunch"", ""AfterLunch"", ""BeforeDinner"", ""AfterDinner"", ""BeforeSleep""]
        For BloodPressure:
          one of [""Morning"", ""Noon"", ""Evening"", ""Afternoon""]
    ""subtype"":
        For BloodGlucose:
          one of [""Fasting"", ""PostPrandial"", ""null""]
        For BloodPressure:
          one of [""Resting"", ""Sitting"", ""Standing"", ""null""]
        For other types: null
    ""reason"": The clinical reason for measurement, written in **Vietnamese**, clearly tailored to the patient's condition, concise, easy to understand, and limited to a maximum of 150 words.
  }},
  ...
]

🚫 Do NOT include any explanation before or after the JSON array.
🚫 RULE: Do NOT allow duplicate combinations of `recordType` and `period`.  
Each `recordType` with a specific `period` can appear only once in the entire array.
---

🧑‍⚕️ PATIENT PROFILE:
- ID: {request.PatientId}
- Age: {request.Age}
- Gender: {request.Gender}
- BMI: {request.Bmi}
- Diabetes type: {request.DiabetesType}
- Insulin schedule: {request.InsulinSchedule}
- Treatment method: {request.TreatmentMethod}
- Complications: {complications}
- Past diseases: {pastDiseases}
- Lifestyle: {request.Lifestyle}

---
Please generate a clinically sound and personalized measurement plan based on this information.";
    }

    public static string BuildPromptAiNote(RequestForAiNoteDto request)
    {
        return
            $@"You are a helpful digital health assistant for diabetic patients. Analyze the following health measurement and return a warm and friendly explanation in Vietnamese.
 
==============================
Input
==============================
Measurement Type: {request.MeasurementType}
Value: {request.Value}
Time: {request.Time}  // in 24h format, e.g., ""07:00"" or ""21:30""
Context (optional): {request.Context}  // e.g., ""fasting"", ""after lunch"", ""resting"", or leave empty
Note: {request.Note}  // Patient’s note like diet, sleep, stress, activities...
 
==============================
Instruction
==============================
1. Determine whether the measurement is high, low, or normal.
2. If context is provided, consider it in your explanation. If empty, skip it.
3. Use the patient’s note to guess possible reasons for the result.
4. Provide kind, encouraging suggestions for next time.
5. Use a warm, conversational, and supportive tone.
6. Write ONLY in Vietnamese.
7. Maximum 250 words.
8. Do NOT mention you are an AI or assistant.
9. Do NOT return any JSON, markdown, or heading — just plain text.
10. Never give strict medical advice — only soft observations or suggestions.
 
==============================
Example Output (in Vietnamese)
==============================
Chỉ số huyết áp của bạn tối nay là 145/90 mmHg, hơi cao hơn mức bình thường. Có thể do bạn đang bị căng thẳng vì công việc và uống cà phê – cả hai yếu tố này đều làm tăng huyết áp tạm thời. Bạn nên thử thư giãn trước khi đo và hạn chế uống cà phê vào buổi tối. Duy trì lối sống lành mạnh sẽ giúp chỉ số ổn định hơn. Bạn đang cố gắng rất tốt rồi, cứ tiếp tục nhé!
 
==============================
Reminder
==============================
No headings. No backticks. Just return plain Vietnamese text only.";
    }
}