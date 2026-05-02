export default function ErrorMessage({ message, onRetry }) {
  return (
    <div className="flex flex-col items-center justify-center py-20 gap-4">
      <div className="w-12 h-12 rounded-full bg-red-100 flex items-center justify-center">
        <svg className="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
      </div>
      <div className="text-center">
        <p className="font-medium text-gray-800 mb-1">Something went wrong</p>
        <p className="text-sm text-gray-500">{message}</p>
      </div>
      {onRetry && (
        <button onClick={onRetry} className="btn-secondary text-sm">
          Try again
        </button>
      )}
    </div>
  )
}
