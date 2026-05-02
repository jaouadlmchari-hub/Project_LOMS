import { useLocation } from 'react-router-dom'

const pageTitles = {
  '/dashboard': 'Dashboard',
  '/employees': 'Employees',
  '/departments': 'Departments',
  '/jobs': 'Jobs',
  '/countries': 'Countries',
  '/salary': 'Salary Records',
  '/applications': 'Applications',
  '/leave-types': 'Leave Types',
  '/leave-balances': 'Leave Balances',
  '/holidays': 'Public Holidays',
  '/users': 'Users',
}

export default function Navbar({ onMenuToggle }) {
  const location = useLocation()
  const basePath = '/' + location.pathname.split('/')[1]
  const title = pageTitles[basePath] || 'HR Dashboard'

  return (
    <header className="bg-white border-b border-gray-200 h-14 flex items-center px-4 gap-3 sticky top-0 z-10">
      <button
        onClick={onMenuToggle}
        className="lg:hidden p-2 rounded-lg text-gray-500 hover:bg-gray-100"
        aria-label="Toggle menu"
      >
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
        </svg>
      </button>
      <h1 className="text-base font-semibold text-gray-800">{title}</h1>
      <div className="ml-auto flex items-center gap-2">
        <div className="text-xs text-gray-400 hidden sm:flex items-center gap-1.5">
          <div className="w-1.5 h-1.5 rounded-full bg-green-500" />
          <span>API Gateway connected</span>
        </div>
      </div>
    </header>
  )
}
