import { createContext, useContext, useState, useCallback, type ReactNode } from "react";
import { SystemErrorModal } from "@/components/common/SystemErrorModal";

interface EvaluatorErrorContextValue {
  showError: (message: string) => void;
}

const EvaluatorErrorContext = createContext<EvaluatorErrorContextValue | null>(null);

export function EvaluatorErrorProvider({ children }: { children: ReactNode }) {
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const showError = useCallback((message: string) => {
    setErrorMessage(message);
  }, []);

  const dismiss = useCallback(() => {
    setErrorMessage(null);
  }, []);

  return (
    <EvaluatorErrorContext.Provider value={{ showError }}>
      {children}
      <SystemErrorModal message={errorMessage} onClose={dismiss} />
    </EvaluatorErrorContext.Provider>
  );
}

export function useEvaluatorError(): EvaluatorErrorContextValue {
  const ctx = useContext(EvaluatorErrorContext);
  if (!ctx) throw new Error("useEvaluatorError must be used inside EvaluatorErrorProvider");
  return ctx;
}
