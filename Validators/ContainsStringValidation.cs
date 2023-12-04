using System.ComponentModel.DataAnnotations;

namespace MyCustomValidators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ContainsStringValidation: ValidationAttribute
    {
        private string MemberName { get; }

        public ContainsStringValidation(string memberName, string? errorMessage = null)
        {
            MemberName = string.IsNullOrEmpty(memberName) ? throw new ArgumentNullException(nameof(memberName)) : memberName;
            ErrorMessage = errorMessage ?? "Valor contido";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var valueString = (string?)value;

            if (string.IsNullOrEmpty(valueString))
                return ValidationResult.Success;

            IEnumerable<string>? memberValue;
            try
            {
                memberValue = (IEnumerable<string>?)validationContext.ObjectType.GetProperty(MemberName)?.GetValue(validationContext.ObjectInstance);

                memberValue ??= (IEnumerable<string>?)validationContext.ObjectType.GetField(MemberName)?.GetValue(validationContext.ObjectInstance);

                if (memberValue == null)
                {
                    throw new ArgumentException($"Não foi possível encontrar o membro {MemberName}. Verifique se ele foi definido.");
                }
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"Não foi possível fazer o cast do membro {MemberName}. O membro dever ser do tipo \"IEnumerable<string>\".", ex);
            }

            return memberValue.Contains(valueString) ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }
    }
}
