import React from 'react'
export function Tag(props: { title: string, description: string }) {
   return (
      <>
         <div className='gap-y-2 flex flex-col p-2 w-fit rounded-sm border-2 border-[#474747] border-solid text-[#e4e4e4] hover:border-[#915eff] transition-all duration-0.25 cursor-pointer' style={{ fontFamily: 'Inter' }}>
            <p className='w-fit'>#{props.title}</p>
            <p className='w-fit'>{props.description}</p>
         </div>
      </>
   )
}
